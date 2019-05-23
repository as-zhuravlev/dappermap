using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using Npgsql;
using Dapper;

using LMS.Core.Entities;
using LMS.Core.Interfaces;

namespace LMS.Postgres.Configuration
{
    public static class PgConfig
    {
        private const string _createDbScriptResourceName = "LMS.Postgres.createdb.pls";

        public static void CreateRelations(string connectionString)
        {
            using (var conn = new NpgsqlConnection())
            {
                conn.ConnectionString = connectionString;

                var assembly = Assembly.GetExecutingAssembly();
                var names = assembly.GetManifestResourceNames();

                var sr = new StreamReader(assembly.GetManifestResourceStream(_createDbScriptResourceName));

                string sql = sr.ReadToEnd();

                conn.Open();
                var pars = ParseConnectionString(connectionString);
                if (pars.ContainsKey("Search Path"))
                {
                    if (conn.ExecuteScalar<string>($"SELECT schema_name FROM information_schema.schemata WHERE schema_name = '{pars["Search Path"]}'") == null)
                    {
                        conn.Execute($"CREATE SCHEMA {pars["Search Path"]}");
                    }
                    
                }

                var com = new NpgsqlCommand(sql, conn);
                com.ExecuteNonQuery();
            }
        }

        public static void SeedData(string connectionString)
        {
            var lectors = new List<Lector>
            {
                new Lector(){ Name = "Albert Einstein",   Email = "albert@ya.ru",      Phone = "+79998887766" },
                new Lector(){ Name = "Erwin Schrödinger", Email = "erwin@gmail.com",   Phone = "+71112223344" },
                new Lector(){ Name = "Paul Dirac",        Email = "dirac777@yahoo.ru", Phone = "+31415926535" },
                new Lector(){ Name = "James Maxwell",     Email = "maxwell@vk.com",    Phone = "+70001122334" },
            };

            var students = new List<Student>()
            {
                new Student() { Name = "Ivan Ivanov" , Email = "ivan@mail.ru", Phone = "+71010010100" },
                new Student() { Name = "Petr Petrov" , Email = "petr@bk.ru"  , Phone = "+71241235425" },
                new Student() { Name = "Sergey Serov", Email = "serov@tr.ru" , Phone = "+71122364345" },
                new Student() { Name = "Nikita Nikov", Email = "nikov@ha.ru" , Phone = "+70101231455" },
                new Student() { Name = "Vova Vovov"  , Email = "vovov@zz.ru" , Phone = "+70134545346" },
            };

            var courses = new List<Course>()
            {
                new Course() { Name = "Physics" },
                new Course() { Name = "Math" },
                new Course() { Name = "Art" },
                new Course() { Name = "Music" },
            };

            using (var conn = new NpgsqlConnection(connectionString))
            {

                SeedEntity(conn, "SELECT * FROM lectors_view", "SELECT * FROM create_person('lectors', @Name, @Email, @Phone)", lectors, l => l.Email);
                SeedEntity(conn, "SELECT * FROM students_view", "SELECT * FROM create_person('students', @Name, @Email, @Phone)", students, s => s.Email);
                int i = 0;
                courses.ForEach((x) => x.LectorId = lectors[i++].Id);
                courses.Last().LectorId = lectors.First().Id; // Albert Einstein was a good musician.

                SeedEntity(conn, "SELECT * FROM courses", "INSERT INTO courses (Name, LectorId) VALUES (@Name, @LectorId) RETURNING Id", courses, c => c.Name);


                var scs = new List<StudentCourse>()
                {
                    new StudentCourse() { CourseId = courses[0].Id, StudentId = students[0].Id },
                    new StudentCourse() { CourseId = courses[1].Id, StudentId = students[0].Id },
                    new StudentCourse() { CourseId = courses[3].Id, StudentId = students[0].Id },
                    new StudentCourse() { CourseId = courses[0].Id, StudentId = students[1].Id },
                    new StudentCourse() { CourseId = courses[0].Id, StudentId = students[2].Id },
                    new StudentCourse() { CourseId = courses[1].Id, StudentId = students[2].Id },
                    new StudentCourse() { CourseId = courses[1].Id, StudentId = students[3].Id },
                    new StudentCourse() { CourseId = courses[1].Id, StudentId = students[4].Id },
                    new StudentCourse() { CourseId = courses[3].Id, StudentId = students[2].Id },
                    new StudentCourse() { CourseId = courses[3].Id, StudentId = students[4].Id },
                };


                SeedEntity(conn, "SELECT * FROM StudentsCourses", "INSERT INTO StudentsCourses (StudentId, CourseId) VALUES (@StudentId, @CourseId) RETURNING Id",
                    scs, sc => sc.CourseId.ToString() + sc.StudentId.ToString());

                var date = new DateTime(2019, 1, 1);

                var lections = new List<Lection>()
                {
                    new Lection() { Name = "Physics Intro", CourseId = courses[0].Id, Date = date.AddDays(0) },
                    new Lection() { Name = "Mechanics", CourseId = courses[0].Id, Date = date.AddDays(2) },
                    new Lection() { Name = "Electromagnetics", CourseId = courses[0].Id, Date = date.AddDays(4) },
                    new Lection() { Name = "Optics", CourseId = courses[0].Id, Date = date.AddDays(3) },
                    new Lection() { Name = "Algebra", CourseId = courses[1].Id, Date = date },
                    new Lection() { Name = "Geometry", CourseId = courses[1].Id, Date = date.AddDays(10) },
                    new Lection() { Name = "Classicism", CourseId = courses[2].Id, Date = date.AddDays(20) },
                    new Lection() { Name = "Modernism", CourseId = courses[2].Id, Date = date.AddDays(0) },
                };

                SeedEntity(conn, "SELECT * FROM lections", "INSERT INTO lections (Name, CourseId, Date) VALUES (@Name, @CourseId, @Date) RETURNING Id",
                    lections, l => l.CourseId.ToString() + l.Name);

                var marks = new List<Mark>()
                {
                    new Mark() { LectionId = lections[0].Id,     StudentCourseId = scs[0].Id, Value =  5},
                    new Mark() { LectionId = lections[1].Id,     StudentCourseId = scs[0].Id, Value = -1},
                    new Mark() { LectionId = lections.Last().Id, StudentCourseId = scs[3].Id, Value =  3},
                    new Mark() { LectionId = lections[0].Id,     StudentCourseId = scs[3].Id, Value =  4},
                };

                SeedEntity(conn, "SELECT * FROM marks", "INSERT INTO marks (Value, LectionId, StudentCourseId) VALUES (@Value, @LectionId, @StudentCourseId) RETURNING Id",
                    marks, m => m.LectionId.ToString() + m.StudentCourseId.ToString());
            }
        }

        static void SeedEntity<T>(NpgsqlConnection conn, string selectSql, string insertSql, IEnumerable<T> entities, Func<T, string> keySelector) where T : IEntity
        {
            var dbEntities = conn.Query<T>(selectSql).ToDictionary(keySelector);
            foreach (var e in entities)
            {
                if (dbEntities.ContainsKey(keySelector(e)))
                    e.Id = dbEntities[keySelector(e)].Id;
                else
                    e.Id = conn.QuerySingle<int>(insertSql, e);
            }

        }

        internal static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            var pairs = connectionString.Split(";");
            var res = new Dictionary<string, string>();
            foreach (var p in pairs)
            {
                var pair = p.Split("=");
                res.Add(pair[0].Trim(), pair[1].Trim());
            }

            return res;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using DapperMap;

using LMS.Core.Interfaces;
using LMS.Core.Entities;
using LMS.Core.Shared;


namespace LMS.Postgres
{
    public class PgRepository : BaseRepository, IRepository
    {
        ILogger _logger;
        
        public PgRepository(IConfiguration conf, ILogger<PgRepository> logger) : base(NpgsqlFactory.Instance, PgRepositoryConf.GetConnectionString(conf)) 
        {
            _logger = logger;
        }
        
        public new int Add<T>(T entity) where T : IEntity, new()
        {
            try
            {
                return base.Add(entity);
            }
            catch (PostgresException ex)
            {
                // see https://www.postgresql.org/docs/9.2/errcodes-appendix.html class 23
                if (ex.SqlState == "23505") 
                {
                    throw new LmsUniqueViolationException($"Violation unique constraint: {ex.Detail}", ex);
                }

                var err = $"Can not add {typeof(T).Name} to Repo";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
            catch (DbException ex)
            {
                var err = $"Can not add {typeof(T).Name} to Repo";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
        }

        public new void Delete<T>(int id) where T : IEntity, new()
        {
            try
            {
                base.Delete<T>(id);
            }
            catch (PostgresException ex)
            {
                var err = $"Can not delete {typeof(T).Name} with id {id} from repo";
                _logger.LogError(ex, err);

                throw new LmsException(err, ex);
            }
        }

        public new IReadOnlyCollection<T> List<T>(
            Expression<Predicate<T>> tWherePredicate) where T : IEntity, new()
        { 
            try
            {
                return base.List(tWherePredicate);
            }
            catch (DbException ex)
            {
                var err = $"Can not list {typeof(T).Name} from repo";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
        }

        
          
        public new IEnumerable<TResult> InnerJoin<T1, T2, TResult> (
           Func<T1, T2, TResult> map,
           Expression<Func<T1, T2, bool>> joinOnPredicate,
           Expression<Func<T1, T2, bool>> wherePredicate)
           where T1 : IEntity, new()
           where T2 : IEntity, new()
        {
            try
            {
                return base.InnerJoin(map, joinOnPredicate, wherePredicate);
            }
            catch (DbException ex)
            {
                var err = $"Can not join {typeof(T1).Name} and {typeof(T2).Name}";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
        }
    
        public new IEnumerable<TResult> InnerJoin<T1, T2, T3, TResult> (
            Func<T1, T2, T3, TResult> map,
            Expression<Func<T1, T2, T3, bool>> joinOnPredicate,
            Expression<Func<T1, T2, T3, bool>> wherePredicate)
            where T1 : IEntity, new()
            where T2 : IEntity, new()
            where T3 : IEntity, new()
        {
            try
            {
                return base.InnerJoin(map, joinOnPredicate, wherePredicate);
            }
            catch (DbException ex)
            {
                var err = $"Can not join {typeof(T1).Name}, {typeof(T2).Name} and {typeof(T3).Name}";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
        }

        public new IEnumerable<TResult> InnerJoin<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> map,
            Expression<Func<T1, T2, T3, T4, bool>> joinOnPredicate,
            Expression<Func<T1, T2, T3, T4, bool>> wherePredicate
            )
            where T1 : IEntity, new()
            where T2 : IEntity, new()
            where T3 : IEntity, new()
            where T4 : IEntity, new()
        {
            try
            {
                return base.InnerJoin(map, joinOnPredicate, wherePredicate);
            }
            catch (DbException ex)
            {
                var err = $"Can not join {typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name} and  {typeof(T4).Name}";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
        }

        public new IEnumerable<TResult> InnerJoin<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, TResult> map,
            Expression<Func<T1, T2, T3, T4, T5, bool>> joinOnPredicate,
            Expression<Func<T1, T2, T3, T4, T5, bool>> wherePredicate
            )
            where T1 : IEntity, new()
            where T2 : IEntity, new()
            where T3 : IEntity, new()
            where T4 : IEntity, new()
            where T5 : IEntity, new()
        {
            try
            {
                return base.InnerJoin(map, joinOnPredicate, wherePredicate);
            }
            catch (DbException ex)
            {
                var err = $"Can not join {typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(T4).Name} and {typeof(T5).Name}";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
        }


        public new void Update<T>(T entity) where T : IEntity, new()
        {
            
            try
            {
                base.Update(entity);
            }
            catch (PostgresException ex)
            {
                // see https://www.postgresql.org/docs/9.2/errcodes-appendix.html class 23
                if (ex.SqlState == "23505")
                {
                    throw new LmsUniqueViolationException($"Violation unique constraint: {ex.Detail}", ex);
                }

                var err = $"Can not update {typeof(T).Name} with id = {entity.Id} ";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
            catch (DbException ex)
            {
                var err = $"Can not update {typeof(T).Name} with id = {entity.Id} ";
                _logger.LogError(ex, err);
                throw new LmsException(err, ex);
            }
        }
        
        protected override void ConfigureMapping()
        {
            AddMapping<Lector>(new SqlMapping
            (
                selectFromRelation: "lectors_view",
                createSql: "SELECT * FROM create_person('lectors', @Name, @Email, @Phone)",
                updateSql: "UPDATE persons SET Name = @Name, Email = @Email, Phone = @Phone WHERE PersonId = @PersonId",
                deleteSql: "PERFORM delete_person('lectors', @Id)"
            ));

            AddMapping<Student>( new SqlMapping
            (
                selectFromRelation: "students_view",
                createSql: "SELECT *  FROM create_person('students', @Name, @Email, @Phone)",
                updateSql: "UPDATE persons SET Name = @Name, Email = @Email, Phone = @Phone WHERE PersonId = @PersonId",
                deleteSql: "SELECT delete_person('students', @Id)"
            ));
            
            AddMapping<Course>(new PgTableMapping<Course>("Courses"));
            AddMapping<Lection>(new PgTableMapping<Lection>("Lections"));
            AddMapping<StudentCourse>(new PgTableMapping<StudentCourse>("StudentsCourses"));
            AddMapping<Mark>(new PgTableMapping<Mark>("Marks"));
        }
    }
}

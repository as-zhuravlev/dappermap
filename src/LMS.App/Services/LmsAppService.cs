using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AutoMapper;

using LMS.Core.Entities;
using LMS.Core.Interfaces;
using LMS.Core.Shared;
using LMS.App.Interfaces;
using LMS.App.ViewModels;
using LMS.App.ViewModelToEntityMappings;
using LMS.App.Helpers;

namespace LMS.App.Services
{
    public class LmsAppService : ILmsAppService
    {
        private readonly IMapper _mapper;
        private readonly IRepository _repository;
        private static readonly Dictionary<Type, IViewModelToEntityMappingBase> _mapDict = new Dictionary<Type, IViewModelToEntityMappingBase>();

        public LmsAppService(IMapper mapper, IRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        static LmsAppService()
        {
            _mapDict.Add(typeof(LectorViewModel), new LectorViewModelToEntityMapping());
            _mapDict.Add(typeof(StudentViewModel), new StudentViewModelToEntityMapping());
            _mapDict.Add(typeof(CourseViewModel), new CourseViewModelToEntityMapping());
            _mapDict.Add(typeof(LectionViewModel), new LectionViewModelToEntityMapping());
        }

        public IReadOnlyCollection<T> List<T>(Expression<Predicate<T>> predicate) where T : CrudViewModel
        {
            var map = GetMapping<T>();

            return map.List(_mapper, _repository, predicate);
        }

        public int Create<T>(T vm) where T : CrudViewModel
        {
            var map = GetMapping<T>();
            
            return map.Create(_mapper, _repository, vm);
        }

        public void Update<T>(T vm) where T : CrudViewModel
        {
            var map = GetMapping<T>();

            map.Update(_mapper, _repository, vm);
        }

        public void Delete<T>(int id) where T : CrudViewModel
        {
            var map = GetMapping<T>();

            map.Delete(_mapper, _repository, id);
        }

        public void EnrollStudentInCourse(int studentId, int courseId)
        {
            _repository.Add(new StudentCourse() { StudentId = studentId, CourseId = courseId });
        }
        
        public void RateStudent(int lectionId, int studentId, string value)
        {
            StudentCourse studentCourse = _repository.InnerJoin((StudentCourse sc, Lection l) => sc, (sc, l) => l.CourseId == sc.CourseId, (sc, l) => sc.StudentId == studentId).FirstOrDefault();

            if (studentCourse == null)
                throw new LmsWithClientMsgException($"Can not rate studentId={studentId} lectionId={lectionId}", $"Can not rate because Student (id={studentId}) is not enrolled in course with Lection (id ={lectionId}).");

            try
            {
                _repository.Add(new Mark() { LectionId = lectionId, StudentCourseId = studentCourse.Id, Value = MarkValueAsShort(value) });
            }
            catch (LmsUniqueViolationException ex)
            {
                throw new LmsWithClientMsgException($"Student's (id={studentId}) homework for lection (id ={lectionId}) has been already rated.", ex);
            }
        }

        public IReadOnlyCollection<MarkViewModel> GetMarks(int? studentId = null, 
                                                           int? courseId = null,
                                                           int? lectionId = null)
        {
            var predicateBuilder = new СonjunctionPredicateBuilder();
            predicateBuilder.AddArg<Mark>();


            if (lectionId.HasValue)
                predicateBuilder.AddArgWithPropertyEqComparation((Lection l) => l.Id, lectionId.Value);
            else
                predicateBuilder.AddArg<Lection>();

            if (courseId.HasValue)
                predicateBuilder.AddArgWithPropertyEqComparation((Course c) => c.Id, courseId.Value);
            else
                predicateBuilder.AddArg<Course>();
            
            predicateBuilder.AddArg<StudentCourse>();
            
            if (studentId.HasValue)
                predicateBuilder.AddArgWithPropertyEqComparation((Student c) => c.Id, studentId.Value);
            else
                predicateBuilder.AddArg<Student>();
            
            var predicate = (predicateBuilder.IsEmpty ? null : (predicateBuilder.Lambda))
                            as Expression<Func<Mark, Lection, Course, StudentCourse, Student, bool>>;

            return _repository.InnerJoin((Mark m, Lection l, Course c, StudentCourse sc, Student s) => new MarkViewModel()
            {
                Id = m.Id,
                Value = MarkValueAsString(m.Value),
                Lection = _mapper.Map<Lection, LectionViewModel>(l),
                Course = _mapper.Map<Course, CourseViewModel>(c),
                Student = _mapper.Map<Student, StudentViewModel>(s)
            }, (m, l, c, sc, s) => m.LectionId == l.Id && l.CourseId == c.Id && c.Id == sc.CourseId && sc.StudentId == s.Id,
               predicate).ToList();
        }

        private IViewModelToEntityMapping<T> GetMapping<T>() where T : CrudViewModel
        {
            var destType = typeof(T);
            if (!_mapDict.ContainsKey(destType))
                throw new InvalidOperationException($"Can not find mapping for {destType.Name}");

            return (IViewModelToEntityMapping<T>) _mapDict[destType];
        }
        
        static private string MarkValueAsString(short mark)
        {
            return mark == Mark.AbsenceValue  ? "Absence" : mark.ToString();
        }

        static private short MarkValueAsShort(string mark)
        {
            if (mark == null)
                throw new ArgumentNullException(nameof(mark));

            mark = mark.Trim();

            if (string.Compare(mark, "Absence", true) == 0)
                return Mark.AbsenceValue;

            return short.Parse(mark);
        }

        public IReadOnlyCollection<StudentViewModel> GetCourseStudents(int courseId)
        {
            return _repository.InnerJoin((sc, s) => s, (StudentCourse sc, Student s) => sc.StudentId == s.Id, (sc, s) => sc.CourseId == courseId)
                         .Select(x => _mapper.Map<Student, StudentViewModel>(x)).ToList();
        }

        public IReadOnlyCollection<CourseViewModel> GetStudentCourses(int studentId)
        {
            return _repository.InnerJoin((sc, s) => s, (StudentCourse sc, Course c) => sc.CourseId == c.Id, (sc, s) => sc.StudentId == studentId)
                    .Select(x => _mapper.Map<Course, CourseViewModel>(x)).ToList();
        }

        public static IEnumerable<Profile> AutoMappersProfiles 
        {
            get
            {
                foreach (var m in _mapDict)
                {
                    foreach (var p in m.Value.AutoMappersProfiles)
                        yield return p;
                }
            }
        }

    }
}

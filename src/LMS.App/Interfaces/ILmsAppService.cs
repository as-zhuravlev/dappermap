using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

using LMS.App.ViewModels;

namespace LMS.App.Interfaces
{
    public interface ILmsAppService
    {
        IReadOnlyCollection<T> List<T>(Expression<Predicate<T>> predicate = null) where T : CrudViewModel;

        int Create<T>(T vm) where T : CrudViewModel;

        void Update<T>(T vm) where T : CrudViewModel;

        void Delete<T>(int id) where T : CrudViewModel;

        void EnrollStudentInCourse(int studentId, int courseId);
        
        IReadOnlyCollection<StudentViewModel> GetCourseStudents(int courseId);

        IReadOnlyCollection<CourseViewModel> GetStudentCourses(int studentId);
        
        void RateStudent(int lectionId, int studentId, string value);

        IReadOnlyCollection<MarkViewModel> GetMarks(int? studentId = null, int? courseId = null, int? lectionId = null);
    }
}

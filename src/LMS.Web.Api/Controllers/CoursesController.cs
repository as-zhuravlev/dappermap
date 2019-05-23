using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using LMS.App.Interfaces;
using LMS.App.ViewModels;
using LMS.Core.Shared;
using LMS.App.Helpers;

namespace LMS.Web.Api.Controllers
{
    public class CoursesController : BaseController<CourseViewModel>
    {
        public CoursesController(ILmsAppService appService, ILogger<LectorsController> logger) : base(appService, logger)
        {
        }

        [HttpGet("search")]
        public IEnumerable<CourseViewModel> Search([FromQuery] string name = null, [FromQuery] int? lectorId = null)
        {
            var predicateBuilder = new СonjunctionPredicateBuilder();
            if (name != null)
                predicateBuilder.AddPropertyEqComparation((CourseViewModel x) => x.Name, name);
            
            if (lectorId != null)
                predicateBuilder.AddPropertyEqComparation((CourseViewModel x) => x.LectorId, lectorId.Value);
            
            if (!predicateBuilder.IsEmpty)
                return _appService.List<CourseViewModel>((Expression<Predicate<CourseViewModel>>)predicateBuilder.Lambda);
            else
                return _appService.List<CourseViewModel>();
        }


        [HttpGet("{id}/students")]
        public IEnumerable<StudentViewModel> GetStudents(int id)
        {
            return _appService.GetCourseStudents(id);
        }
        
        [HttpPost("enroll")]
        public IActionResult EnrollStudent([FromQuery] int courseId, [FromQuery] int studentId)
        {
            try
            {
                _appService.EnrollStudentInCourse(studentId, courseId);
                return Accepted();
            }
            catch (LmsWithClientMsgException ex)
            {
                return Conflict(ex.ClientMessage);
            }
        }
    }
}



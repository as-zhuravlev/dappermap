using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

using LMS.App.Interfaces;
using LMS.App.ViewModels;
using LMS.App.Helpers;

namespace LMS.Web.Api.Controllers
{
    public class StudentsController : BaseController<StudentViewModel>
    {
        public StudentsController(ILmsAppService appService, ILogger<LectorsController> logger) : base(appService, logger)
        {
        }

        [HttpGet("search")]
        public IEnumerable<StudentViewModel> Search([FromQuery] string name = null, [FromQuery] string email = null, [FromQuery] string phone = null)
        {
            var predicateBuilder = new СonjunctionPredicateBuilder();
            if (name != null)
                predicateBuilder.AddPropertyEqComparation((StudentViewModel x) => x.Name, name);

            if (email != null)
                predicateBuilder.AddPropertyEqComparation((StudentViewModel x) => x.Email, email);

            if (phone != null)
                predicateBuilder.AddPropertyEqComparation((StudentViewModel x) => x.Email, phone);

            if (!predicateBuilder.IsEmpty)
                return _appService.List((Expression<Predicate<StudentViewModel>>)predicateBuilder.Lambda);
            else
                return _appService.List<StudentViewModel>();
        }

        
        [HttpGet("{id}/courses")]
        public IEnumerable<CourseViewModel> GetCourses(int id)
        {
            return _appService.GetStudentCourses(id);
        }
    }
}
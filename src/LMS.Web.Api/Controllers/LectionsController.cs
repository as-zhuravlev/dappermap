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
    public class LectionsController : BaseController<LectionViewModel>
    {
        public LectionsController(ILmsAppService appService, ILogger<LectorsController> logger) : base(appService, logger)
        {
        }

        [HttpGet("search")]
        public IEnumerable<LectionViewModel> Search([FromQuery] string name = null, [FromQuery] int? courseId = null, [FromQuery] DateTime? date = null)
        {
            var predicateBuilder = new СonjunctionPredicateBuilder();
            if (name != null)
                predicateBuilder.AddPropertyEqComparation((LectionViewModel x) => x.Name, name);

            if (courseId != null)
                predicateBuilder.AddPropertyEqComparation((LectionViewModel x) => x.CourseId, courseId.Value);

            if (date != null)
                predicateBuilder.AddPropertyEqComparation((LectionViewModel x) => x.Date, date.Value);
            
            if (!predicateBuilder.IsEmpty)
                return _appService.List<LectionViewModel>((Expression<Predicate<LectionViewModel>>)predicateBuilder.Lambda);
            else
                return _appService.List<LectionViewModel>();
        }
    }
}

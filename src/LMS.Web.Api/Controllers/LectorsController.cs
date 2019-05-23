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
    public class LectorsController : BaseController<LectorViewModel>
    {
        public LectorsController(ILmsAppService appService, ILogger<LectorsController> logger) : base(appService, logger)
        {
        }

        [HttpGet("search")]
        public IEnumerable<LectorViewModel> Search([FromQuery] string name = null, [FromQuery] string email = null, [FromQuery] string phone = null)
        {
            var predicateBuilder = new СonjunctionPredicateBuilder();
            if (name != null)
                predicateBuilder.AddPropertyEqComparation((LectorViewModel x) => x.Name, name);

            if (email != null)
                predicateBuilder.AddPropertyEqComparation((LectorViewModel x) => x.Email, email);

            if (phone != null)
                predicateBuilder.AddPropertyEqComparation((LectorViewModel x) => x.Email, phone);
            
            if (!predicateBuilder.IsEmpty)
                return _appService.List<LectorViewModel>((Expression<Predicate<LectorViewModel>>)predicateBuilder.Lambda);
            else
                return _appService.List<LectorViewModel>();
        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using LMS.App.Interfaces;
using LMS.App.ViewModels;
using LMS.Core.Shared;
using System.ComponentModel.DataAnnotations;

namespace LMS.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarksController : ControllerBase
    {
        protected readonly ILmsAppService _appService;
        protected readonly ILogger _logger;

        public MarksController(ILmsAppService appService, ILogger<MarksController> logger)
        {
            _appService = appService;
            _logger = logger;
        }

        [HttpPost("rate")]
        public IActionResult Rate([FromQuery, Required] int lectionId, [FromQuery, Required] int studentId, [FromQuery, Required, RegularExpression("^([0-5]|Absence)$")] string value)
        {
            try
            {
                _appService.RateStudent(lectionId, studentId, value);
                return Accepted();
            }
            catch (LmsWithClientMsgException ex)
            {
                return Conflict(ex.ClientMessage);
            }
        }

        [HttpGet]
        public IEnumerable<MarkViewModel> Get(int? studentId = null, int? courseId = null, int? lectionId = null)
        {
            return _appService.GetMarks(studentId, courseId, lectionId);
        }
    }
}
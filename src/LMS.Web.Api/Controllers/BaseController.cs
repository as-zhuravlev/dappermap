using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

using LMS.App.Interfaces;
using LMS.App.ViewModels;
using LMS.Core.Shared;
using LMS.Web.Api.ActionFilters;

namespace LMS.Web.Api.Controllers
{
    [Route("api/[controller]")]
    public class BaseController<T> : ControllerBase where T : CrudViewModel
    {
        protected readonly ILmsAppService _appService;
        protected readonly ILogger _logger;
        
        public BaseController(ILmsAppService appService, ILogger logger)
        {
            _appService = appService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<T> GetAll()
        {
            return _appService.List<T>();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var vm = _appService.List<T>(x => x.Id == id).FirstOrDefault();
        
            if (vm != null)
                return Ok(vm);
            else
                return new NotFoundObjectResult($"Can not find {typeof(T).Name.Replace("ViewModel", string.Empty)} with id = {id}");
        }
        
        [HttpPost]
        [ValidateViewModelState]
        public IActionResult Create([FromBody] T mv)
        {
            try
            {
                mv.Id = _appService.Create(mv);
            }
            catch (LmsUniqueViolationException ex)
            {
                return Conflict(ex.Message);
            }

            return CreatedAtAction(nameof(GetById), new { id = mv.Id }, mv);
        }
        
        [HttpPatch]
        public IActionResult Edit([FromBody] T mv)
        {
            try
            {
                _appService.Update(mv);
                return Ok(mv);
            }
            catch (LmsUniqueViolationException ex)
            {
                return Conflict(ex.Message);
            }
            
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery, Required] int id)
        {
             _appService.Delete<T>(id);

            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Contracts;
using System.Net;

namespace LMS.Presentation.Controllers;

[Route("api/courses")]
[ApiController]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(IServiceManager serviceManager, ILogger<CoursesController> logger)
    {
        _serviceManager = serviceManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courseDtos = await _serviceManager.CourseService.GetAllCourses();
        //var json = JsonConvert.SerializeObject(courseDtos);
        return Ok(courseDtos);
    }

    [HttpGet("course/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try {
            var courseDto = await _serviceManager.CourseService.GetCourseById(id);
            var json = JsonConvert.SerializeObject(courseDto);
            return Ok(courseDto);
        } catch (Exception ex) {
            _logger.LogCritical("Error occurred: {ErrorMessage}", ex.Message);
            throw;
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("user/{id:guid}")]
    public async Task<IActionResult> GetByUserId(Guid id)
    {
        try {
            var courseDtos = _serviceManager.CourseService.GetCourseByUserId(id);
            return Ok(courseDtos);
        } catch (Exception ex) {
            _logger.LogCritical("Error occurred: {ErrorMessage}", ex.Message);
            throw;
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}

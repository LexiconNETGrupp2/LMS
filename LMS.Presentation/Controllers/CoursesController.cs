using LMS.Shared.Constants;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Contracts;
using System.Net;
using System.Security.Claims;

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
        return Ok(courseDtos);
    }

    [HttpGet("course/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var courseDto = await _serviceManager.CourseService.GetCourseById(id);
        if (courseDto is null) {
            return BadRequest();
        }
        return Ok(courseDto);
    }

    [HttpGet("user/{id:guid}")]
    public async Task<IActionResult> GetByUserId(Guid id)
    {
        // If a student is requesting someone else's courses, return 401
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isStudent = User.IsInRole(RolesNames.Student);
        if (isStudent &&
            (currentUserId is null || 
            currentUserId != id.ToString())) 
        {
            return Unauthorized();
        }

        var courseDto = await _serviceManager.CourseService.GetCourseByUserId(id);
        if (isStudent && courseDto.Count != 1) {
            return BadRequest();
        }
        return Ok(courseDto);
    }
}

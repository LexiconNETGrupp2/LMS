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
        // If a student is requesting all courses, return 401
        if (IsStudent()) {
            return Unauthorized();
        }

        var courseDtos = await _serviceManager.CourseService.GetAllCourses();
        return Ok(courseDtos);
    }

    [HttpGet("course/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // If a student is requesting someone else's courses, return 401
        if (IsStudentGettingUnauthorizedCourse(id)) {
            return Unauthorized();
        }

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
        if (IsStudentGettingUnauthorizedCourse(id)) 
        {
            return Unauthorized();
        }

        var courseDto = await _serviceManager.CourseService.GetCourseByUserId(id);
        if (IsStudent() && courseDto.Count != 1) {
            _logger.LogError("Student {StudentId} is enrolled in 0 or >1 courses", id);
            return BadRequest();
        }
        return Ok(courseDto);
    }

    private bool IsStudent()
        => User.IsInRole(RolesNames.Student);

    private bool IsStudentGettingUnauthorizedCourse(Guid studentId)
    {
        var isStudent = IsStudent();
        if (!isStudent) return false;
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return currentUserId is null || currentUserId != studentId.ToString();
    }
}

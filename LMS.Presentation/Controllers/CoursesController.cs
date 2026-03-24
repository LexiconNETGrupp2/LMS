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
    public async Task<IActionResult> GetAll([FromQuery] AllCoursesParams param, CancellationToken token)
    {
        // If a student is requesting all courses, return 401
        if (IsStudent()) {
            return Unauthorized();
        }

        var courseDtos = await _serviceManager.CourseService.GetAllCourses(param, token);
        return Ok(courseDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        // If a student is requesting a course they're not in, return 401
        string? currentStudentId = null;
        if (IsStudent()) currentStudentId = GetCurrentUserId();

        var courseDto = await _serviceManager.CourseService.GetCourseById(id, currentStudentId, token);
        if (courseDto is null) {
            return BadRequest();
        }
        return Ok(courseDto);
    }

    [HttpGet("user/{id:guid}")]
    public async Task<IActionResult> GetByUserId(Guid id, CancellationToken token)
    {
        // If a student is requesting someone else's courses, return 401
        if (IsStudentGettingUnauthorizedCourse(id)) 
        {
            return Unauthorized();
        }

        var courseDto = await _serviceManager.CourseService.GetCourseByUserId(id, token);
        return Ok(courseDto);
    }

    [HttpGet("me/participants")]
    public async Task<IActionResult> GetMyCourseParticipants(CancellationToken token)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var dto = await _serviceManager.CourseService.GetCourseParticipantsByUserId(userId, token);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    private bool IsStudent()
        => User.IsInRole(RolesNames.Student);
    private string? GetCurrentUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private bool IsStudentGettingUnauthorizedCourse(Guid studentId)
    {
        var isStudent = IsStudent();
        if (!isStudent) return false;
        var currentUserId = GetCurrentUserId();
        return currentUserId is null || currentUserId != studentId.ToString();
    }
}

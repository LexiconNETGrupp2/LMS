using LMS.Shared.Constants;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;
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
    [Authorize(Roles = RolesNames.Teacher)]
    [SwaggerOperation(
        Summary = "Gets all the courses. Requires Teacher role",
        Description = "Gets all the courses that's in the database"
    )]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "You need to be a teacher")]
    public async Task<IActionResult> GetAll([FromQuery] AllCoursesParams param, CancellationToken token)
    {
        var courseDtos = await _serviceManager.CourseService.GetAllCourses(param, token);
        return Ok(courseDtos);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get a course by its ID",
        Description = "Get's a course by its ID. If a student is requesting a course they're not in, returns a 401"
    )]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "You need to be a teacher to request a course you're not in")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken token)
    {
        // If a student is requesting a course they're not in, return 401
        string? currentStudentId = null;
        if (IsStudent()) currentStudentId = GetCurrentUserId();

        var courseDto = await _serviceManager.CourseService.GetCourseById(id, currentStudentId, token);
        if (courseDto is null) {
            return NotFound();
        }
        return Ok(courseDto);
    }

    [HttpGet("user/{id:guid}")]
    [SwaggerOperation(
        Summary = "Get a course by a user's ID",
        Description = "Takes a user's ID and returns the course they're in. " +
            "Returns 401 if a student is trying to request someone else's course (ID of authorized user != ID in request)"
    )]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "You can only request your own course")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByUserId(Guid id, CancellationToken token)
    {
        // If a student is requesting someone else's courses, return 401
        if (IsStudentGettingUnauthorizedCourse(id)) 
        {
            return Unauthorized();
        }

        var courseDto = await _serviceManager.CourseService.GetCourseByUserId(id, token);
        if (courseDto is null) {
            return NotFound();
        }
        return Ok(courseDto);
    }

    [HttpPost("create")]
    [Authorize(Roles = RolesNames.Teacher)]
    [SwaggerOperation(
        Summary = "Add a course to the database",
        Description = ""
    )]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto, CancellationToken token)
    {
        var result = await _serviceManager.CourseService.CreateCourse(createCourseDto, token);
        return result ? Created() : BadRequest();
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

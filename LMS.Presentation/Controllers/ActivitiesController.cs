using LMS.Shared.Constants;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ActivitiesController(IServiceManager serviceManager) : ControllerBase
{
    private IActivityService ActivityService => serviceManager.ActivityService;

    [HttpGet]
    [Authorize(Roles = RolesNames.Teacher)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [SwaggerOperation(
        Summary = "Get all activities",
        Description = "Retrieves all activities in the system."
    )]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAllActivities([FromQuery] PagedQuery query)
    {
        var activities = await ActivityService.GetAllActivities(query);
        return Ok(activities);
    }

    [HttpGet("{id:guid}", Name = nameof(GetActivityById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get activity by ID",
        Description = "Retrieves a specific activity by its unique identifier."
    )]
    public async Task<ActionResult<ActivityDto>> GetActivityById(Guid id)
    {
        var activity = await ActivityService.GetActivityById(id);
        return Ok(activity);
    }

    [HttpGet("module/{moduleId:guid}", Name = nameof(GetActivitiesByModuleId))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get activities by module ID",
        Description = "Retrieves all activities associated with a specific module."
    )]
    public async Task<ActionResult<ActivityDto>> GetActivitiesByModuleId(Guid moduleId)
    {
        var activities = await ActivityService.GetActivitiesFromModuleId(moduleId);
        return Ok(activities);
    }

    [HttpPost]
    [Authorize(Roles = RolesNames.Teacher)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Create a new activity",
        Description = "Creates a new activity based on the provided data."
    )]
    public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto request)
    {
        var response = await ActivityService.CreateActivity(request);
        return CreatedAtAction(nameof(GetActivityById), new { response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RolesNames.Teacher)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update an activity",
        Description = "Updates an existing activity. Requires Teacher role."
    )]
    public async Task<IActionResult> UpdateActivity(Guid id, [FromBody] UpdateActivityDto request)
    {
        await ActivityService.UpdateActivity(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RolesNames.Teacher)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete an activity",
        Description = "Deletes an existing activity. Requires Teacher role."
    )]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        await ActivityService.DeleteActivity(id);
        return NoContent();
    }
}

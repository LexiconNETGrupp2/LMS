using LMS.Shared.DTOs;
using LMS.Shared.DTOs.ActivityDtos;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController(IServiceManager serviceManager) : ControllerBase
{
    private IActivityService ActivityService => serviceManager.ActivityService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAllActivities()
    {
        var activities = await ActivityService.GetAllActivities();
        return Ok(activities);
    }

    [HttpGet("{id:guid}", Name = nameof(GetActivityById))]
    public async Task<ActionResult<ActivityDto>> GetActivityById(Guid id)
    {
        var activity = await ActivityService.GetActivityById(id);
        if (activity == null)
            return NotFound();
        return Ok(activity);
    }

    [HttpGet("module/{moduleId:guid}", Name = nameof(GetActivitiesByModuleId))]
    public async Task<ActionResult<ActivityDto>> GetActivitiesByModuleId(Guid moduleId)
    {
        var activities = await ActivityService.GetActivitiesFromModuleId(moduleId);
        return Ok(activities);
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivity([FromBody] CreateActivityDto request)
    {
        var response = await ActivityService.CreateActivity(request);
        return CreatedAtAction(nameof(GetActivityById), new { response.Id }, response);
    }
}

using Microsoft.AspNetCore.Mvc;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateActivity()
    {
        return Created("api/activities", null);
    }
}

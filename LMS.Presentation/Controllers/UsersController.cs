using LMS.Shared.Constants;
using LMS.Shared.DTOs.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RolesNames.Teacher)]
public class UsersController(IServiceManager serviceManager) : ControllerBase
{
    private IUserService UserService => serviceManager.UserService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<UserDto>>> GetAll()
    {
        var users = await UserService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var user = await UserService.GetUserById(id);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await UserService.DeleteUser(id);
        return NoContent();
    }
}

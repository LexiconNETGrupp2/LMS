using LMS.Shared.Constants;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = RolesNames.Teacher)]
public class UsersController(IServiceManager serviceManager) : ControllerBase
{
    private IUserService UserService => serviceManager.UserService;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAll([FromQuery] AllUsersParams query)
    {
        var users = await UserService.GetAllUsers(query);
        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(UserRegistrationDto request)
    {
        var user = await UserService.CreateUser(request);
        return CreatedAtAction(nameof(GetById), new { user.Id }, user);
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

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto request, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);

        var updatedUser = await UserService.UpdateUser(id, request, token);
        return Ok(updatedUser);
    }
}

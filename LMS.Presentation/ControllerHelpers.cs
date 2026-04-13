using LMS.Shared.Constants;
using System.Security.Claims;

namespace LMS.Presentation;

public class ControllerHelpers
{
    public bool IsStudent(ClaimsPrincipal user)
        => user.IsInRole(RolesNames.Student);
    public string? GetCurrentUserId(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier);
}

using LMS.Shared.Constants;
using System.Security.Claims;

namespace LMS.Presentation;

public class ControllerHelpers
{
    public static bool IsStudent(ClaimsPrincipal user)
        => user.IsInRole(RolesNames.Student);
    public static string? GetCurrentUserId(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier);
}

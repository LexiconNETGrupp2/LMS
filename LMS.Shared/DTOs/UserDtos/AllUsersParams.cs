using LMS.Shared.DataObjects;

namespace LMS.Shared.DTOs.UserDtos;

public sealed record AllUsersParams(
    string? Role
) : SearchAndSortParam;

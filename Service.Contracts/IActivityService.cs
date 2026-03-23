using LMS.Shared.DTOs;
using LMS.Shared.DTOs.ActivityDtos;

namespace Service.Contracts;

public interface IActivityService
{
    Task<ActivityDto> CreateActivity(CreateActivityDto request);
}

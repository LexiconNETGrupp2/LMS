using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.Pagination;

namespace Service.Contracts;

public interface IActivityService
{
    Task<ActivityDto> CreateActivity(CreateActivityDto request);
    Task<ActivityDto?> GetActivityById(Guid id);
    Task<PagedResult<ActivityDto>> GetAllActivities(PagedQuery query);
    Task<List<ActivityDto>> GetActivitiesFromModuleId(Guid moduleId);
}

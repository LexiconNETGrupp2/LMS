using Domain.Models.Entities;
using LMS.Shared.Pagination;

namespace Domain.Contracts.Repositories;

public interface IActivityRepository : IRepositoryBase<Activity>
{
    public Task<IReadOnlyCollection<Activity>> GetActivitiesFromModuleId(Guid moduleId);
    public Task<Activity?> GetActivityById(Guid id, bool trackChanges = false);
    public Task<PagedResult<Activity>> GetAllActivities(PagedQuery query);
}

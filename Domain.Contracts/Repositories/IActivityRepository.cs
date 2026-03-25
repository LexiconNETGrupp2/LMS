using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IActivityRepository : IRepositoryBase<Activity>
{
    public Task<IReadOnlyCollection<Activity>> GetActivitiesFromModuleId(Guid moduleId);
    public Task<Activity?> GetActivityById(Guid id, bool trackChanges = false);
    public Task<List<Activity>> GetAllActivities();
}

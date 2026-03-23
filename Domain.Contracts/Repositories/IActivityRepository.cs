using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IActivityRepository : IRepositoryBase<Activity>
{
    public IReadOnlyCollection<Activity> GetActivitiesFromModuleId(Guid moduleId);
    public Activity? GetActivityById(Guid id);
}

using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;

public class ActivityRepository(ApplicationDbContext context)
    : RepositoryBase<Activity>(context), IActivityRepository
{
    public IReadOnlyCollection<Activity> GetActivitiesFromModuleId(Guid moduleId)
    {
        return FindAll()
            .Where(s => s.Module.Id == moduleId)
            .ToList();
    }

    public Activity? GetActivityById(Guid id)
    {
        return FindByCondition(s => s.Id == id).FirstOrDefault();
    }
}

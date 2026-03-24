using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ActivityRepository(ApplicationDbContext context)
    : RepositoryBase<Activity>(context), IActivityRepository
{
    public async Task<IReadOnlyCollection<Activity>> GetActivitiesFromModuleId(Guid moduleId)
    {
        return FindAll()
            .Where(s => s.Module.Id == moduleId)
            .ToList();
    }

    public async Task<Activity?> GetActivityById(Guid id)
    {
        return FindByCondition(s => s.Id == id).FirstOrDefault();
    }

    public async Task<List<Activity>> GetAllActivities()
    {
        return await FindAll().ToListAsync();
    }
}

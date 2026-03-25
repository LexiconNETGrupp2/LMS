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
            .Include(s => s.Module)
            .Include(s => s.Type)
            .ToList();
    }

    public async Task<Activity?> GetActivityById(Guid id, bool trackChanges)
    {
        return FindByCondition(s => s.Id == id, trackChanges)
            .Include(s => s.Module)
            .Include(s => s.Type)
            .FirstOrDefault();
    }

    public async Task<List<Activity>> GetAllActivities()
    {
        return await FindAll()
            .Include(s => s.Module)
            .Include(s => s.Type)
            .ToListAsync();
    }
}

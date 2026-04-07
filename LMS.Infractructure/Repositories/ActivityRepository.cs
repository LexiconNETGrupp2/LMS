using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Infractructure.Extensions;
using LMS.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ActivityRepository(ApplicationDbContext context)
    : RepositoryBase<Activity>(context), IActivityRepository
{
    public async Task<IReadOnlyCollection<Activity>> GetActivitiesFromModuleId(Guid moduleId)
    {
        return await FindAll()
            .Where(s => s.Module.Id == moduleId)
            .Include(s => s.Module)
            .Include(s => s.Type)
            .ToListAsync();
    }

    public async Task<Activity?> GetActivityById(Guid id, bool trackChanges)
    {
        return await FindByCondition(s => s.Id == id, trackChanges)
            .Include(s => s.Module)
            .Include(s => s.Type)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResult<Activity>> GetAllActivities(PagedQuery query)
    {
        return await FindAll()
            .Include(s => s.Module)
            .Include(s => s.Type)
            .ToPagedResultAsync(query);
    }
}

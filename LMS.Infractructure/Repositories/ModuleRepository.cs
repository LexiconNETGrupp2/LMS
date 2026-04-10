using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Infractructure.Extensions;
using LMS.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ModuleRepository(ApplicationDbContext context)
    : RepositoryBase<Module>(context), IModuleRepository
{
    public async Task<PagedResult<Module>> GetAllModulesAsync(PagedQuery query)
    {
        return await FindAll()
            .Include(m => m.Course)
            .OrderBy(m => m.StartDate)
            .ToPagedResultAsync(query);
    }

    public async Task<Module?> GetModuleByIdAsync(Guid id)
    {
        return await FindAll()
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IReadOnlyCollection<Module>> GetModulesByCourseIdAsync(Guid courseId)
    {
        return await FindAll()
            .Include(m => m.Course)
            .Where(m => m.Course.Id == courseId)
            .OrderBy(m => m.StartDate)
            .ToListAsync();
    }

    public async Task<Module?> GetModuleByIdTrackedAsync(Guid id)
    {
        return await FindAll(trackChanges: true)
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}

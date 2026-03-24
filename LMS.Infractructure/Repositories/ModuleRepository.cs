using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ModuleRepository : RepositoryBase<Module>, IModuleRepository
{
    private readonly ApplicationDbContext _context;

    public ModuleRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Module>> GetAllModulesAsync()
    {
        return await _context.Modules
            .AsNoTracking()
            .Include(m => m.Course)
            .ToListAsync();
    }

    public async Task<Module?> GetModuleByIdAsync(Guid id)
    {
        return await _context.Modules
            .AsNoTracking()
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IReadOnlyCollection<Module>> GetModulesByCourseIdAsync(Guid courseId)
    {
        return await _context.Modules
            .AsNoTracking()
            .Include(m => m.Course)
            .Where(m => m.Course.Id == courseId)
            .ToListAsync();
    }

    public async Task<Module?> GetModuleByIdTrackedAsync(Guid id)
    {
        return await _context.Modules
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
}

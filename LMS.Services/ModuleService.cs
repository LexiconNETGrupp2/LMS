
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Infractructure.Data;
using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;

namespace LMS.Services;

public class ModuleService : IModuleService
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public ModuleService(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
    {
        return await context.Modules
            .Include(m => m.Course)
            .ProjectTo<ModuleDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(Guid id)
    {
        return await context.Modules
            .Include(m => m.Course)
            .Where(m => m.Id == id)
            .ProjectTo<ModuleDto>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<ModuleDto>> GetModulesByCourseIdAsync(Guid courseId)
    {
        return await context.Modules
            .Include(m => m.Course)
            .Where(m => m.Course.Id == courseId)
            .ProjectTo<ModuleDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ModuleDto> CreateModuleAsync(CreateModuleDto createModuleDto)
    {
        var course = await context.Courses.FindAsync(createModuleDto.CourseId);
        if (course == null)
            throw new KeyNotFoundException("Course not found");

        var module = new Domain.Models.Entities.Module
        {
            Name = createModuleDto.Name,
            StartDate = DateOnly.FromDateTime(createModuleDto.StartDate),
            EndDate = DateOnly.FromDateTime(createModuleDto.EndDate),
            Course = course
        };

        context.Modules.Add(module);
        await context.SaveChangesAsync();

        return mapper.Map<ModuleDto>(module);
    }

    public async Task UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto)
    {
        var module = await context.Modules.FindAsync(id);
        if (module == null)
            throw new KeyNotFoundException("Module not found");

        module.Name = updateModuleDto.Name;
        module.StartDate = DateOnly.FromDateTime(updateModuleDto.StartDate);
        module.EndDate = DateOnly.FromDateTime(updateModuleDto.EndDate);

        await context.SaveChangesAsync();
    }

    public async Task DeleteModuleAsync(Guid id)
    {
        var module = await context.Modules.FindAsync(id);
        if (module == null)
            throw new KeyNotFoundException("Module not found");

        context.Modules.Remove(module);
        await context.SaveChangesAsync();
    }
}

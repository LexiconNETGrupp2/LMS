
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;

namespace LMS.Services;

public class ModuleService : IModuleService
{
    private readonly IUnitOfWork _uow;
    private readonly IModuleRepository _moduleRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public ModuleService(
        IUnitOfWork uow,
        IModuleRepository moduleRepository,
        ICourseRepository courseRepository,
        IMapper mapper)
    {
        _uow = uow;
        _moduleRepository = moduleRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
    {
        var modules = await _moduleRepository.FindAll()
            .Include(m => m.Course)
            .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return modules;
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(Guid id)
    {
        var module = await _moduleRepository.FindByCondition(m => m.Id == id)
            .Include(m => m.Course)
            .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();

        return module;
    }

    public async Task<IEnumerable<ModuleDto>> GetModulesByCourseIdAsync(Guid courseId)
    {
        var modules = await _moduleRepository.FindByCondition(m => m.Course.Id == courseId)
            .Include(m => m.Course)
            .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return modules;
    }

    public async Task<ModuleDto> CreateModuleAsync(CreateModuleDto createModuleDto)
    {
        var course = await _courseRepository.GetCourseById(createModuleDto.CourseId, CancellationToken.None);
        if (course == null)
            throw new KeyNotFoundException("Course not found");

        var module = new Module
        {
            Name = createModuleDto.Name,
            StartDate = createModuleDto.StartDate,
            EndDate = createModuleDto.EndDate,
            Course = course
        };

        _moduleRepository.Create(module);
        await _uow.CompleteAsync();

        return _mapper.Map<ModuleDto>(module);
    }

    public async Task UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto)
    {
        var module = await _moduleRepository.FindByCondition(m => m.Id == id, trackChanges: true)
            .FirstOrDefaultAsync();

        if (module == null)
            throw new KeyNotFoundException("Module not found");

        module.Name = updateModuleDto.Name;
        module.StartDate = updateModuleDto.StartDate;
        module.EndDate = updateModuleDto.EndDate;

        await _uow.CompleteAsync();
    }

    public async Task DeleteModuleAsync(Guid id)
    {
        var module = await _moduleRepository.FindByCondition(m => m.Id == id, trackChanges: true)
            .FirstOrDefaultAsync();

        if (module == null)
            throw new KeyNotFoundException("Module not found");

        _moduleRepository.Delete(module);
        await _uow.CompleteAsync();
    }
}

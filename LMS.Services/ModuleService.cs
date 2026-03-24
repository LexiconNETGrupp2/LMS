
using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs.ModuleDtos;
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
        var modules = await _moduleRepository.GetAllModulesAsync();
        return _mapper.Map<IEnumerable<ModuleDto>>(modules);
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(Guid id)
    {
        var module = await _moduleRepository.GetModuleByIdAsync(id);
        return module is null ? null : _mapper.Map<ModuleDto>(module);
    }

    public async Task<IEnumerable<ModuleDto>> GetModulesByCourseIdAsync(Guid courseId)
    {
        var modules = await _moduleRepository.GetModulesByCourseIdAsync(courseId);
        return _mapper.Map<IEnumerable<ModuleDto>>(modules);
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
        var module = await _moduleRepository.GetModuleByIdTrackedAsync(id);
        if (module == null)
            throw new KeyNotFoundException("Module not found");

        module.Name = updateModuleDto.Name;
        module.StartDate = updateModuleDto.StartDate;
        module.EndDate = updateModuleDto.EndDate;

        await _uow.CompleteAsync();
    }

    public async Task DeleteModuleAsync(Guid id)
    {
        var module = await _moduleRepository.GetModuleByIdTrackedAsync(id);
        if (module == null)
            throw new KeyNotFoundException("Module not found");

        _moduleRepository.Delete(module);
        await _uow.CompleteAsync();
    }
}

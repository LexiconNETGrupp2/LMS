
using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
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
        var course = await _courseRepository.GetCourseByIdTracked(createModuleDto.CourseId, CancellationToken.None);
        if (course == null)
            throw new NotFoundException("Course not found.");

        ValidateName(createModuleDto.Name);
        ValidateDateRange(createModuleDto.StartDate, createModuleDto.EndDate);
        ValidateModuleWithinCourse(createModuleDto.StartDate, createModuleDto.EndDate, course);
        await EnsureModuleDoesNotOverlapAsync(
            createModuleDto.CourseId,
            createModuleDto.StartDate,
            createModuleDto.EndDate);

        var module = new Module
        {
            Name = createModuleDto.Name,
            StartDate = createModuleDto.StartDate,
            EndDate = createModuleDto.EndDate,
            Course = course
        };

        _moduleRepository.Create(module);
        await _uow.CompleteAsync(CancellationToken.None);

        return _mapper.Map<ModuleDto>(module);
    }

    public async Task UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto)
    {
        var module = await _moduleRepository.GetModuleByIdTrackedAsync(id);
        if (module == null)
            throw new NotFoundException("Module not found.");

        ValidateName(updateModuleDto.Name);
        ValidateDateRange(updateModuleDto.StartDate, updateModuleDto.EndDate);
        ValidateModuleWithinCourse(updateModuleDto.StartDate, updateModuleDto.EndDate, module.Course);
        await EnsureModuleDoesNotOverlapAsync(
            module.Course.Id,
            updateModuleDto.StartDate,
            updateModuleDto.EndDate,
            module.Id);

        module.Name = updateModuleDto.Name;
        module.StartDate = updateModuleDto.StartDate;
        module.EndDate = updateModuleDto.EndDate;

        await _uow.CompleteAsync(CancellationToken.None);
    }

    public async Task DeleteModuleAsync(Guid id)
    {
        var module = await _moduleRepository.GetModuleByIdTrackedAsync(id);
        if (module == null)
            throw new NotFoundException("Module not found.");

        _moduleRepository.Delete(module);
        await _uow.CompleteAsync(CancellationToken.None);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Module name is required.");
        }
    }

    private static void ValidateDateRange(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
        {
            throw new BadRequestException("Module start date cannot be later than the end date.");
        }
    }

    private static void ValidateModuleWithinCourse(DateOnly startDate, DateOnly endDate, Course course)
    {
        if (startDate < course.StartDate || endDate > course.EndDate)
        {
            throw new BadRequestException(
                $"Module dates must stay within the course dates ({course.StartDate:yyyy-MM-dd} - {course.EndDate:yyyy-MM-dd}).");
        }
    }

    private async Task EnsureModuleDoesNotOverlapAsync(
        Guid courseId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? currentModuleId = null)
    {
        var existingModules = await _moduleRepository.GetModulesByCourseIdAsync(courseId);

        var hasOverlap = existingModules.Any(module =>
            module.Id != currentModuleId &&
            startDate <= module.EndDate &&
            endDate >= module.StartDate);

        if (hasOverlap)
        {
            throw new BadRequestException("Module dates overlap with another module in this course.");
        }
    }
}

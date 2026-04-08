using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.Pagination;
using Service.Contracts;

namespace LMS.Services;

public class ActivityService : IActivityService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ActivityService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ActivityDto?> GetActivityById(Guid id)
    {
        var activity = await _uow.Activities.GetActivityById(id)
            ?? throw new ActivityNotFoundException(id);
        return _mapper.Map<ActivityDto>(activity);
    }

    public async Task<PagedResult<ActivityDto>> GetAllActivities(PagedQuery query)
    {
        var result = await _uow.Activities.GetAllActivities(query);
        return new PagedResult<ActivityDto>
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            Items = _mapper.Map<List<ActivityDto>>(result.Items),
        };
    }

    public async Task<List<ActivityDto>> GetActivitiesFromModuleId(Guid moduleId)
    {
        var activities = await _uow.Activities.GetActivitiesFromModuleId(moduleId);
        return _mapper.Map<List<ActivityDto>>(activities);
    }

    public async Task<ActivityDto> CreateActivity(CreateActivityDto request)
    {
        var module = await _uow.Modules.GetModuleByIdTrackedAsync(request.ModuleId)
            ?? throw new ModuleNotFoundException(request.ModuleId);
        var activityType = _uow.ActivityTypes.FirstOrDefault(a => a.Name == request.Type.Name)
            ?? throw new ActivityTypeNotFoundException(request.Type.Name);

        ValidateDateRange(request.StartDate, request.EndDate);
        ValidateActivityWithinModule(request.StartDate, request.EndDate, module);
        await EnsureActivityDoesNotOverlapAsync(request.ModuleId, request.StartDate, request.EndDate);

        var activity = new Activity
        {
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = activityType,
            Module = module,
        };
        try {
            _uow.Activities.Create(activity);
            await _uow.CompleteAsync(CancellationToken.None);
            return _mapper.Map<ActivityDto>(activity);
        } catch (Exception ex) {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task UpdateActivity(Guid id, UpdateActivityDto request)
    {
        var activity = await _uow.Activities.GetActivityById(id, trackChanges: true)
            ?? throw new ActivityNotFoundException(id);
        var activityType = _uow.ActivityTypes.FirstOrDefault(a => a.Name == request.Type.Name)
            ?? throw new ActivityTypeNotFoundException(request.Type.Name);

        ValidateDateRange(request.StartDate, request.EndDate);
        ValidateActivityWithinModule(request.StartDate, request.EndDate, activity.Module);
        await EnsureActivityDoesNotOverlapAsync(activity.ModuleId, request.StartDate, request.EndDate, activity.Id);

        activity.Name = request.Name;
        activity.Description = request.Description;
        activity.StartDate = request.StartDate;
        activity.EndDate = request.EndDate;
        activity.Type = activityType;

        try {
            _uow.Activities.Update(activity);
            await _uow.CompleteAsync(CancellationToken.None);
        } catch (Exception ex) {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task DeleteActivity(Guid id)
    {
        var activity = await _uow.Activities.GetActivityById(id, trackChanges: true)
            ?? throw new ActivityNotFoundException(id);

        try {
            _uow.Activities.Delete(activity);
            await _uow.CompleteAsync(CancellationToken.None);
        } catch (Exception ex) {
            throw new BadRequestException(ex.Message);
        }
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            throw new BadRequestException("Aktivitetens startdatum kan inte vara senare än slutdatum.");
        }
    }

    private static void ValidateActivityWithinModule(DateTime startDate, DateTime endDate, Module module)
    {
        if (DateOnly.FromDateTime(startDate) < module.StartDate ||
            DateOnly.FromDateTime(endDate) > module.EndDate)
        {
            throw new BadRequestException(
                $"Aktivitetens datum måste ligga inom modulens datum ({module.StartDate:yyyy-MM-dd} - {module.EndDate:yyyy-MM-dd}).");
        }
    }

    private async Task EnsureActivityDoesNotOverlapAsync(
        Guid moduleId,
        DateTime startDate,
        DateTime endDate,
        Guid? currentActivityId = null)
    {
        var existingActivities = await _uow.Activities.GetActivitiesFromModuleId(moduleId);

        var hasOverlap = existingActivities.Any(activity =>
            activity.Id != currentActivityId &&
            startDate < activity.EndDate &&
            endDate > activity.StartDate);

        if (hasOverlap)
        {
            throw new BadRequestException("Aktivitetens datum överlappar med en annan aktivitet i den här modulen.");
        }
    }
}

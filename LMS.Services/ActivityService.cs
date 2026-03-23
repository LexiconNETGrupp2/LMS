using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Shared.DTOs.CourseDtos;
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

    public async Task<ActivityDto> CreateActivity(CreateActivityDto request)
    {
        var activity = _mapper.Map<Activity>(request);
        try {
            _uow.Activities.Create(activity);
            await _uow.CompleteAsync(CancellationToken.None);
            return _mapper.Map<ActivityDto>(activity);
        } catch (Exception ex) {
            throw new Exception($"Error creating activity: {ex.Message}");
        }
    }
}

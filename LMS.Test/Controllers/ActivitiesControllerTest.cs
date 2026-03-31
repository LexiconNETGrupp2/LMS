using Domain.Models.Exceptions;
using LMS.Presentation.Controllers;
using LMS.Shared;
using LMS.Shared.DTOs.ActivityDtos;
using LMS.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Contracts;

namespace LMS.Test.Controllers;

public class ActivitiesControllerTest
{
    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetAll_ReturnsOkWithActivities()
    {
        // Arrange
        Guid moduleId = Guid.NewGuid();
        List<ActivityDto> expectedActivities = [
                ActivityHelpers.GenerateActivityDto(moduleId),
                ActivityHelpers.GenerateActivityDto(moduleId)
            ];
        var query = new PagedQuery { Page = 1, PageSize = 10 };
        Mock<IActivityService> activityServiceMock = new();
        activityServiceMock
            .Setup(s => s.GetAllActivities(query))
            .ReturnsAsync(new PagedResult<ActivityDto>
            {
                Page = expectedActivities.Count,
                PageSize =  query.PageSize,
                TotalItems = expectedActivities.Count,
                Items = expectedActivities,
            });

        ActivitiesController activityController = ActivityHelpers.CreateController(activityServiceMock);

        // Act
        var result = await activityController.GetAllActivities(query);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PagedResult<ActivityDto>>(okResult.Value);
        Assert.Same(expectedActivities, response.Items);
        activityServiceMock.Verify(s => s.GetAllActivities(query), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetActivityById_ReturnsOkWithActivity()
    {
        ActivityDto activityDto = ActivityHelpers.GenerateActivityDto();
        Guid activityId = activityDto.Id;
        Mock<IActivityService> activityServiceMock = new();
        activityServiceMock
            .Setup(s => s.GetActivityById(activityId))
            .ReturnsAsync(activityDto);
        ActivitiesController controller = ActivityHelpers.CreateController(activityServiceMock);

        var response = await controller.GetActivityById(activityId);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Equal(activityDto, okResult.Value);
        activityServiceMock.Verify(s => s.GetActivityById(activityId), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetActivityById_Whenunknown_ReturnsNotFound()
    {
        Guid id = Guid.NewGuid();
        Mock<IActivityService> activityServiceMock = new();
        activityServiceMock
            .Setup(s => s.GetActivityById(id))
            .ReturnsAsync((ActivityDto?)null);
        
        ActivitiesController controller = ActivityHelpers.CreateController(activityServiceMock);

        var response = await controller.GetActivityById(id);

        var notFoundResult = Assert.IsType<NotFoundResult>(response.Result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        activityServiceMock.Verify(s => s.GetActivityById(id), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetActivitiesByModuleId_WhenCalled_ReturnsOkWithActivities()
    {
        Guid moduleId = Guid.NewGuid();
        List<ActivityDto> activities = [
            ActivityHelpers.GenerateActivityDto(moduleId),
            ActivityHelpers.GenerateActivityDto(moduleId),
            ActivityHelpers.GenerateActivityDto(moduleId)
        ];
        Mock<IActivityService> activityServiceMock = new();
        activityServiceMock
            .Setup(s => s.GetActivitiesFromModuleId(moduleId))
            .ReturnsAsync(activities);
        ActivitiesController controller = ActivityHelpers.CreateController(activityServiceMock);

        var reponse = await controller.GetActivitiesByModuleId(moduleId);

        var okResult = Assert.IsType<OkObjectResult>(reponse.Result);
        Assert.Same(activities, okResult.Value);
        activityServiceMock.Verify(s => s.GetActivitiesFromModuleId(moduleId), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task CreateActivity_WhenValid_ReturnsCreated()
    {
        Guid moduleId = Guid.NewGuid();
        CreateActivityDto createActivityDto = ActivityHelpers.GenerateCreateActivityDto(moduleId);
        ActivityDto activityDto = ActivityHelpers.GenerateActivityDto(moduleId);
        Mock<IActivityService> activityServiceMock = new();
        activityServiceMock
            .Setup(s => s.CreateActivity(createActivityDto))
            .ReturnsAsync(activityDto);
        ActivitiesController controller = ActivityHelpers.CreateController(activityServiceMock);

        var response = await controller.CreateActivity(createActivityDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(response);
        Assert.Equal(activityDto, createdResult.Value);
        activityServiceMock.Verify(s => s.CreateActivity(createActivityDto), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task CreateActivity_WhenModuleNotFound_ThrowsNotFoundException()
    {
        CreateActivityDto createActivityDto = ActivityHelpers.GenerateCreateActivityDto();
        Mock<IActivityService> activityServiceMock = new();
        activityServiceMock
            .Setup(s => s.CreateActivity(createActivityDto))
            .Throws(new NotFoundException("Module not found"));
        ActivitiesController controller = ActivityHelpers.CreateController(activityServiceMock);

        await Assert.ThrowsAsync<NotFoundException>(
            async () => await controller.CreateActivity(createActivityDto)
        );
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task CreateActivity_WhenInvalid_ThrowsException()
    {
        CreateActivityDto createActivityDto = ActivityHelpers.GenerateCreateActivityDto();
        Mock<IActivityService> activityServiceMock = new();
        activityServiceMock
            .Setup(s => s.CreateActivity(createActivityDto))
            .Throws(new Exception("Something went wrong"));
        ActivitiesController controller = ActivityHelpers.CreateController(activityServiceMock);

        await Assert.ThrowsAsync<Exception>(
            async () => await controller.CreateActivity(createActivityDto)
        );
    }
}

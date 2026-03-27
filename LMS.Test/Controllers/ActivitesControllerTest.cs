using Domain.Models.Entities;
using LMS.Presentation.Controllers;
using LMS.Shared.DTOs.ActivityDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Service.Contracts;
using System.Security.Claims;

namespace LMS.Test.Controllers;

public class ActivitesControllerTest
{
    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetAll_ReturnsOkWithActivities()
    {
        // Arrange
        Guid moduleId = Guid.NewGuid();
        List<ActivityDto> expectedActivities = [
                new(
                    Id: Guid.NewGuid(),
                    Name: "aktivitet 1",
                    Description: "aaa",
                    StartDate: DateTime.Now,
                    EndDate: DateTime.Now.AddHours(2),
                    Type: new(Name: "Document"),
                    ModuleId: moduleId
                ),
                new(
                    Id: Guid.NewGuid(),
                    Name: "aktivitet 2",
                    Description: "bbb",
                    StartDate: DateTime.Now.AddHours(2),
                    EndDate: DateTime.Now.AddHours(4),
                    Type: new(Name: "Document"),
                    ModuleId: moduleId
                )
            ];
        Mock<IActivityService> activityService = new();
        activityService
            .Setup(a => a.GetAllActivities())
            .ReturnsAsync(expectedActivities);
        
        var activityController = CreateController(activityService);

        // Act
        var result = await activityController.GetAllActivities();
        
        // Assert
        Assert.IsType<ActionResult<IEnumerable<ActivityDto>>>(result);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
    } 

    private static ActivitiesController CreateController(
        Mock<IActivityService> activityServiceMock,
        ClaimsPrincipal? user = null)
    {
        var serviceManagerMock = new Mock<IServiceManager>();
        serviceManagerMock.SetupGet(s => s.ActivityService).Returns(activityServiceMock.Object);

        var controller = new ActivitiesController(serviceManagerMock.Object) {
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext {
                    User = user ?? new ClaimsPrincipal(new ClaimsIdentity()),
                }
            },
        };

        return controller;
    }
}

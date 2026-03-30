using LMS.Presentation.Controllers;
using LMS.Shared.DTOs.ActivityDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Contracts;
using System.Security.Claims;

namespace LMS.Test.Helpers;

public static class ActivityHelpers
{
    public static ActivitiesController CreateController(
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

    public static ActivityDto GenerateActivityDto() => GenerateActivityDto(Guid.NewGuid());
    public static ActivityDto GenerateActivityDto(Guid moduleId)
        => new(
            Id: Guid.NewGuid(),
            Name: "Lecture 3",
            Description: "Lecture about lecturing",
            StartDate: new DateTime(2026, 1, 1, 10, 15, 0),
            EndDate: new DateTime(2026, 1, 1, 12, 0, 0),
            Type: new(Name: "Lecture"),
            ModuleId: moduleId
        );

    public static CreateActivityDto GenerateCreateActivityDto() => GenerateCreateActivityDto(Guid.NewGuid());
    public static CreateActivityDto GenerateCreateActivityDto(Guid moduleId)
        => new(
            Name: "Lecture 3",
            Description: "Lecture about lecturing",
            StartDate: new DateTime(2026, 1, 1, 10, 15, 0),
            EndDate: new DateTime(2026, 1, 1, 12, 0, 0),
            Type: new(Name: "Lecture"),
            ModuleId: moduleId
        );
}

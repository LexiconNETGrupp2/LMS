using System.Security.Claims;
using LMS.Presentation.Controllers;
using LMS.Shared.Constants;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Service.Contracts;

namespace LMS.Test.Controllers;

public class CoursesControllerTest
{
    [Fact]
    public async Task GetAll_ReturnsOkWithCourses()
    {
        // Arrange
        var ct = CancellationToken.None;
        var param = new AllCoursesParams(
            Search: null,
            AfterDate: null,
            BeforeDate: null
        );

        var courseServiceMock = new Mock<ICourseService>();
        IReadOnlyCollection<CourseDto> expectedCourses = [];
        courseServiceMock
            .Setup(s => s.GetAllCourses(param, ct))
            .ReturnsAsync(expectedCourses);

        var controller = CreateController(courseServiceMock);

        // Act
        var result = await controller.GetAll(param, ct);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expectedCourses, okResult.Value);
        courseServiceMock.Verify(s => s.GetAllCourses(param, ct), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenCourseIsMissing_ReturnsNotFound()
    {
        // Arrange
        var ct = CancellationToken.None;
        var courseId = Guid.NewGuid();

        var courseServiceMock = new Mock<ICourseService>();
        courseServiceMock
            .Setup(s => s.GetCourseById(courseId, null, ct))
            .ReturnsAsync((CourseDto?)null);

        var controller = CreateController(courseServiceMock);

        // Act
        var result = await controller.GetById(courseId, ct);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        courseServiceMock.Verify(s => s.GetCourseById(courseId, null, ct), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenRequestingStudentCourse_ReturnsOkWithCourse()
    {
        // Arrange
        var ct = CancellationToken.None;
        var courseId = Guid.NewGuid();
        var currentStudentId = Guid.NewGuid();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, currentStudentId.ToString()),
            new Claim(ClaimTypes.Role, RolesNames.Student),
        ], "TestAuthType"));

        var courseServiceMock = new Mock<ICourseService>();
        courseServiceMock
            .Setup(s => s.GetCourseById(courseId, currentStudentId.ToString(), ct))
            .ReturnsAsync(new CourseDto
            {
                Id = courseId,
                Name = "Test Course",
                Description = "A course for testing",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(1),
                Modules = [],
            });

        var controller = CreateController(courseServiceMock, principal);

        // Act
        var result = await controller.GetById(courseId, ct);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<CourseDto>(okResult.Value);
        Assert.Equal(courseId, returnValue.Id);
        courseServiceMock.Verify(s => s.GetCourseById(courseId, currentStudentId.ToString(), ct), Times.Once);
    }

    [Fact]
    public async Task GetByUserId_WhenStudentRequestsDifferentUser_ReturnsUnauthorized()
    {
        // Arrange
        var ct = CancellationToken.None;
        var requestedUserId = Guid.NewGuid();
        var currentStudentId = Guid.NewGuid();

        var courseServiceMock = new Mock<ICourseService>();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, currentStudentId.ToString()),
            new Claim(ClaimTypes.Role, RolesNames.Student),
        ], "TestAuthType"));

        var controller = CreateController(courseServiceMock, principal);

        // Act
        var result = await controller.GetByUserId(requestedUserId, ct);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
        courseServiceMock.Verify(s => s.GetCourseByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static CoursesController CreateController(
        Mock<ICourseService> courseServiceMock,
        ClaimsPrincipal? user = null)
    {
        var serviceManagerMock = new Mock<IServiceManager>();
        serviceManagerMock.SetupGet(s => s.CourseService).Returns(courseServiceMock.Object);

        var logger = Mock.Of<ILogger<CoursesController>>();
        var controller = new CoursesController(serviceManagerMock.Object, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user ?? new ClaimsPrincipal(new ClaimsIdentity()),
                }
            },
        };

        return controller;
    }
}

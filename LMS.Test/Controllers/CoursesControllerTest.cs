using System.Security.Claims;
using LMS.Presentation.Controllers;
using LMS.Shared.Constants;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Service.Contracts;

namespace LMS.Test.Controllers;

public class CoursesControllerTest
{
    [Fact]
    [Trait("Layer", "Controller")]
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
    [Trait("Layer", "Controller")]
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
    [Trait("Layer", "Controller")]
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
    [Trait("Layer", "Controller")]
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

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task Create_WhenValid_ReturnsCreated()
    {
        // Arrange
        var ct = CancellationToken.None;
        var createCourseDto = new CreateCourseDto {
            Name = "New Course",
            Description = "A new course for testing",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(1),
            Modules = []
        };

        var courseServiceMock = new Mock<ICourseService>();
        courseServiceMock
            .Setup(s => s.CreateCourse(createCourseDto, ct))
            .ReturnsAsync(true);

        var controller = CreateController(courseServiceMock);

        // Act
        var result = await controller.Create(createCourseDto, ct);

        // Assert
        Assert.IsType<CreatedResult>(result);
        courseServiceMock.Verify(s => s.CreateCourse(createCourseDto, ct), Times.Once);
    }

    [Theory]
    [Trait("Layer", "Controller")]
    [InlineData(true, StatusCodes.Status204NoContent)]
    [InlineData(false, StatusCodes.Status404NotFound)]
    public async Task Delete_WhenCalled_ReturnsExpectedStatusResult(bool deleteSucceeded, int expectedStatusCode)
    {
        // Arrange
        var ct = CancellationToken.None;
        var courseId = Guid.NewGuid();

        var courseServiceMock = new Mock<ICourseService>();
        courseServiceMock
            .Setup(s => s.DeleteCourse(courseId, ct))
            .ReturnsAsync(deleteSucceeded);

        var controller = CreateController(courseServiceMock);

        // Act
        var result = await controller.Delete(courseId, ct);

        // Assert
        var statusResult = Assert.IsAssignableFrom<IStatusCodeActionResult>(result);
        Assert.Equal(expectedStatusCode, statusResult.StatusCode);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task Update_WhenUpdateSucceeds_ReturnsNoContent()
    {
        // Arrange
        var ct = CancellationToken.None;
        var courseId = Guid.NewGuid();
        var updateCourseDto = new UpdateCourseDto(
            Name: "Updated Course",
            Description: "An updated course for testing",
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(1)
        );

        var courseServiceMock = new Mock<ICourseService>();
        courseServiceMock
            .Setup(s => s.UpdateCourse(courseId, updateCourseDto, ct))
            .ReturnsAsync(true);

        var controller = CreateController(courseServiceMock);

        // Act
        var result = await controller.Update(courseId, updateCourseDto, ct);

        // Assert
        Assert.IsType<NoContentResult>(result);
        courseServiceMock.Verify(s => s.UpdateCourse(courseId, updateCourseDto, ct), Times.Once);
    }

    [Theory]
    [Trait("Layer", "Controller")]
    [InlineData("invalid-guid", false, StatusCodes.Status401Unauthorized)]
    [InlineData("11111111-1111-1111-1111-111111111111", false, StatusCodes.Status404NotFound)]
    [InlineData("11111111-1111-1111-1111-111111111111", true, StatusCodes.Status200OK)]
    public async Task GetMyCourseParticipants_WhenCalled_ReturnsExpectedStatusCode(
        string userIdClaim,
        bool hasParticipants,
        int expectedStatusCode)
    {
        // Arrange
        var ct = CancellationToken.None;
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userIdClaim),
            new Claim(ClaimTypes.Role, RolesNames.Student),
        ], "TestAuthType"));

        var courseServiceMock = new Mock<ICourseService>();
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            courseServiceMock
                .Setup(s => s.GetCourseParticipantsByUserId(userId, ct))
                .ReturnsAsync(hasParticipants
                    ? new CourseParticipantsDto
                    {
                        Name = "Test Course",
                        Description = "Participants",
                        Students = [],
                    }
                    : null);
        }

        var controller = CreateController(courseServiceMock, principal);

        // Act
        var result = await controller.GetMyCourseParticipants(ct);

        // Assert
        var statusResult = Assert.IsAssignableFrom<IStatusCodeActionResult>(result);
        Assert.Equal(expectedStatusCode, statusResult.StatusCode);
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

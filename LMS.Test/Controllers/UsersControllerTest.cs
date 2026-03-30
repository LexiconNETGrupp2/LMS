using LMS.Presentation.Controllers;
using LMS.Shared.DTOs.UserDtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Contracts;

namespace LMS.Test.Controllers;

public class UsersControllerTest
{
    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        // Arrange
        var ct = CancellationToken.None;
        var userServiceMock = new Mock<IUserService>();
        List<UserDto> expectedUsers = [
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "Testsson",
            },
        ];
        userServiceMock
            .Setup(s => s.GetAllUsers(ct))
            .ReturnsAsync(expectedUsers);

        var controller = CreateController(userServiceMock);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedUsers, okResult.Value);
        userServiceMock.Verify(s => s.GetAllUsers(ct), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task Delete_WhenUserExists_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        var userServiceMock = new Mock<IUserService>();
        userServiceMock
            .Setup(s => s.DeleteUser(userId))
            .Returns(Task.CompletedTask);

        var controller = CreateController(userServiceMock);

        // Act
        var result = await controller.Delete(userId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        userServiceMock.Verify(s => s.DeleteUser(userId), Times.Once);
    }

    private static UsersController CreateController(Mock<IUserService> userServiceMock)
    {
        var serviceManagerMock = new Mock<IServiceManager>();
        serviceManagerMock.SetupGet(m => m.UserService).Returns(userServiceMock.Object);
        return new UsersController(serviceManagerMock.Object);
    }
}

using LMS.Presentation.Controllers;
using LMS.Shared.Constants;
using LMS.Shared.DTOs.AuthDtos;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Pagination;
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
        var expectedUsers = new PagedResult<UserDto>
        {
            Page = 1,
            PageSize = 20,
            TotalItems = 1,
            Items = new List<UserDto>()
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "test@example.com",
                    FirstName = "Test",
                    LastName = "Testsson",
                },
            },
        };
        AllUsersParams query = new(RolesNames.Teacher) { Page = 1, PageSize = 20 };
        userServiceMock
            .Setup(s => s.GetAllUsers(query, ct))
            .ReturnsAsync(expectedUsers);

        var controller = CreateController(userServiceMock);

        // Act
        var result = await controller.GetAll(query);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedUsers, okResult.Value);
        userServiceMock.Verify(s => s.GetAllUsers(query, ct), Times.Once);
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

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task Create_WhenUserIsValid_ReturnsCreatedAtAction()
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var userRegistrationDto = new UserRegistrationDto
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test.testsson@example.com",
            Password = "abc123!",
            Role = "Student",
            CourseId = Guid.NewGuid(),
        };
        var createdUser = new UserDto
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = userRegistrationDto.FirstName,
            LastName = userRegistrationDto.LastName,
            Email = userRegistrationDto.Email,
        };
        userServiceMock
            .Setup(s => s.CreateUser(userRegistrationDto))
            .ReturnsAsync(createdUser);

        var controller = CreateController(userServiceMock);

        // Act
        var result = await controller.Create(userRegistrationDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(UsersController.GetById), createdAtActionResult.ActionName);
        Assert.Equal(createdUser.Id, createdAtActionResult.RouteValues!["id"]);
        Assert.Same(createdUser, createdAtActionResult.Value);
        userServiceMock.Verify(s => s.CreateUser(userRegistrationDto), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task Update_WhenUserIsValid_ReturnsOkWithUpdatedUser()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var ct = new CancellationTokenSource().Token;
        var updateUserDto = new UpdateUserDto
        {
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User"
        };
        var updatedUser = new UserDto
        {
            Id = userId,
            Email = updateUserDto.Email,
            FirstName = updateUserDto.FirstName,
            LastName = updateUserDto.LastName
        };

        var userServiceMock = new Mock<IUserService>();
        userServiceMock
            .Setup(s => s.UpdateUser(userId, updateUserDto, ct))
            .ReturnsAsync(updatedUser);

        var controller = CreateController(userServiceMock);

        // Act
        var result = await controller.Update(userId, updateUserDto, ct);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(updatedUser, okResult.Value);
        userServiceMock.Verify(s => s.UpdateUser(userId, updateUserDto, ct), Times.Once);
    }

    private static UsersController CreateController(Mock<IUserService> userServiceMock)
    {
        var serviceManagerMock = new Mock<IServiceManager>();
        serviceManagerMock.SetupGet(m => m.UserService).Returns(userServiceMock.Object);
        return new UsersController(serviceManagerMock.Object);
    }

}

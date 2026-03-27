using LMS.Presentation.Controllers;
using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Contracts;

namespace LMS.Test.Controllers;

public class ModulesControllerTest
{
    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetAllModules_ReturnsOkWithModules()
    {
        // Arrange
        var expectedModules = new List<ModuleDto>
        {
            new(
                Guid.NewGuid(),
                "Module 1",
                "Desc 1",
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
                Guid.NewGuid(),
                "Course 1")
        };

        var moduleServiceMock = new Mock<IModuleService>();
        moduleServiceMock
            .Setup(s => s.GetAllModulesAsync())
            .ReturnsAsync(expectedModules);

        var controller = CreateController(moduleServiceMock);

        // Act
        var result = await controller.GetAllModules();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedModules, okResult.Value);
        moduleServiceMock.Verify(s => s.GetAllModulesAsync(), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetModuleById_WhenMissing_ReturnsNotFound()
    {
        // Arrange
        var moduleId = Guid.NewGuid();

        var moduleServiceMock = new Mock<IModuleService>();
        moduleServiceMock
            .Setup(s => s.GetModuleByIdAsync(moduleId))
            .ReturnsAsync((ModuleDto?)null);

        var controller = CreateController(moduleServiceMock);

        // Act
        var result = await controller.GetModuleById(moduleId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        moduleServiceMock.Verify(s => s.GetModuleByIdAsync(moduleId), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetModuleById_WhenFound_ReturnsOkWithModule()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var module = new ModuleDto(
            moduleId,
            "Module 1",
            "Desc 1",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
            Guid.NewGuid(),
            "Course 1");

        var moduleServiceMock = new Mock<IModuleService>();
        moduleServiceMock
            .Setup(s => s.GetModuleByIdAsync(moduleId))
            .ReturnsAsync(module);

        var controller = CreateController(moduleServiceMock);

        // Act
        var result = await controller.GetModuleById(moduleId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(module, okResult.Value);
        moduleServiceMock.Verify(s => s.GetModuleByIdAsync(moduleId), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task GetModulesByCourse_ReturnsOkWithModules()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expectedModules = new List<ModuleDto>
        {
            new(
                Guid.NewGuid(),
                "Module 1",
                "Desc 1",
                DateOnly.FromDateTime(DateTime.UtcNow),
                DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
                courseId,
                "Course 1")
        };

        var moduleServiceMock = new Mock<IModuleService>();
        moduleServiceMock
            .Setup(s => s.GetModulesByCourseIdAsync(courseId))
            .ReturnsAsync(expectedModules);

        var controller = CreateController(moduleServiceMock);

        // Act
        var result = await controller.GetModulesByCourse(courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expectedModules, okResult.Value);
        moduleServiceMock.Verify(s => s.GetModulesByCourseIdAsync(courseId), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task CreateModule_WhenValid_ReturnsCreatedAtAction()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var moduleId = Guid.NewGuid();
        var createDto = new CreateModuleDto(
            "New Module",
            "New Description",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(14),
            courseId);

        var createdModule = new ModuleDto(
            moduleId,
            createDto.Name,
            createDto.Description,
            createDto.StartDate,
            createDto.EndDate,
            courseId,
            "Course 1");

        var moduleServiceMock = new Mock<IModuleService>();
        moduleServiceMock
            .Setup(s => s.CreateModuleAsync(createDto))
            .ReturnsAsync(createdModule);

        var controller = CreateController(moduleServiceMock);

        // Act
        var result = await controller.CreateModule(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ModulesController.GetModuleById), createdResult.ActionName);
        Assert.Equal(moduleId, createdResult.RouteValues?["id"]);
        Assert.Same(createdModule, createdResult.Value);
        moduleServiceMock.Verify(s => s.CreateModuleAsync(createDto), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task UpdateModule_WhenCalled_ReturnsNoContent()
    {
        // Arrange
        var moduleId = Guid.NewGuid();
        var updateDto = new UpdateModuleDto(
            "Updated Module",
            "Updated Description",
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10));

        var moduleServiceMock = new Mock<IModuleService>();
        moduleServiceMock
            .Setup(s => s.UpdateModuleAsync(moduleId, updateDto))
            .Returns(Task.CompletedTask);

        var controller = CreateController(moduleServiceMock);

        // Act
        var result = await controller.UpdateModule(moduleId, updateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        moduleServiceMock.Verify(s => s.UpdateModuleAsync(moduleId, updateDto), Times.Once);
    }

    [Fact]
    [Trait("Layer", "Controller")]
    public async Task DeleteModule_WhenCalled_ReturnsNoContent()
    {
        // Arrange
        var moduleId = Guid.NewGuid();

        var moduleServiceMock = new Mock<IModuleService>();
        moduleServiceMock
            .Setup(s => s.DeleteModuleAsync(moduleId))
            .Returns(Task.CompletedTask);

        var controller = CreateController(moduleServiceMock);

        // Act
        var result = await controller.DeleteModule(moduleId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        moduleServiceMock.Verify(s => s.DeleteModuleAsync(moduleId), Times.Once);
    }

    private static ModulesController CreateController(Mock<IModuleService> moduleServiceMock)
    {
        var serviceManagerMock = new Mock<IServiceManager>();
        serviceManagerMock.SetupGet(s => s.ModuleService).Returns(moduleServiceMock.Object);

        return new ModulesController(serviceManagerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };
    }
}

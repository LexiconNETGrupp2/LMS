using LMS.Shared.DTOs.ModuleDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Presentation.Controllers;

[Route("api/modules")]
[ApiController]
[Authorize]
public class ModulesController(IServiceManager serviceManager) : ControllerBase
{
    private readonly IServiceManager _serviceManager = serviceManager;
    private IModuleService moduleService => _serviceManager.ModuleService;
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all modules",
        Description = "Retrieves all modules in the system."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "List of modules", typeof(IEnumerable<ModuleDto>))]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAllModules()
    {
        var modules = await moduleService.GetAllModulesAsync();
        return Ok(modules);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get module by id",
        Description = "Retrieves a single module by its identifier."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Module found", typeof(ModuleDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    public async Task<ActionResult<ModuleDto>> GetModuleById(Guid id)
    {
        var module = await moduleService.GetModuleByIdAsync(id);
        if (module == null)
            return NotFound();

        return Ok(module);
    }

    [HttpGet("course/{courseId:guid}")]
    [SwaggerOperation(
        Summary = "Get modules by course",
        Description = "Retrieves all modules that belong to the specified course."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "List of modules", typeof(IEnumerable<ModuleDto>))]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesByCourse(Guid courseId)
    {
        var modules = await moduleService.GetModulesByCourseIdAsync(courseId);
        return Ok(modules);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(
        Summary = "Create a new module",
        Description = "Creates a new module for the specified course. Requires Teacher role."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Module created", typeof(ModuleDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found")]
    public async Task<ActionResult<ModuleDto>> CreateModule(CreateModuleDto createModuleDto)
    {
        try
        {
            var module = await moduleService.CreateModuleAsync(createModuleDto);
            return CreatedAtAction(nameof(GetModuleById), new { id = module.Id }, module);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(
        Summary = "Update a module",
        Description = "Updates an existing module. Requires Teacher role."
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Module updated")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    public async Task<IActionResult> UpdateModule(Guid id, UpdateModuleDto updateModuleDto)
    {
        try
        {
            await moduleService.UpdateModuleAsync(id, updateModuleDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Teacher")]
    [SwaggerOperation(
        Summary = "Delete a module",
        Description = "Deletes an existing module. Requires Teacher role."
    )]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Module deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Module not found")]
    public async Task<IActionResult> DeleteModule(Guid id)
    {
        try
        {
            await moduleService.DeleteModuleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
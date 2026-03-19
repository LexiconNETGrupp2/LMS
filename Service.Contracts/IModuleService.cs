using LMS.Shared.DTOs.ModuleDtos;

namespace Service.Contracts;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>> GetAllModulesAsync();
    Task<ModuleDto?> GetModuleByIdAsync(Guid id);
    Task<IEnumerable<ModuleDto>> GetModulesByCourseIdAsync(Guid courseId);
    Task<ModuleDto> CreateModuleAsync(CreateModuleDto createModuleDto);
    Task UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto);
    Task DeleteModuleAsync(Guid id);
}

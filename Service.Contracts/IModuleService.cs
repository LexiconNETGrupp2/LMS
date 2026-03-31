using LMS.Shared.DTOs.ModuleDtos;
using LMS.Shared.Pagination;

namespace Service.Contracts;

public interface IModuleService
{
    Task<PagedResult<ModuleDto>> GetAllModulesAsync(PagedQuery query);
    Task<ModuleDto?> GetModuleByIdAsync(Guid id);
    Task<IEnumerable<ModuleDto>> GetModulesByCourseIdAsync(Guid courseId);
    Task<ModuleDto> CreateModuleAsync(CreateModuleDto createModuleDto);
    Task UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto);
    Task DeleteModuleAsync(Guid id);
}

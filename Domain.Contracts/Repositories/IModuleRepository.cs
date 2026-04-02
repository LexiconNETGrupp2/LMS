using Domain.Models.Entities;
using LMS.Shared.Pagination;

namespace Domain.Contracts.Repositories;

public interface IModuleRepository : IRepositoryBase<Module>, IInternalRepositoryBase<Module>
{
    Task<PagedResult<Module>> GetAllModulesAsync(PagedQuery query);
    Task<Module?> GetModuleByIdAsync(Guid id);
    Task<IReadOnlyCollection<Module>> GetModulesByCourseIdAsync(Guid courseId);
    Task<Module?> GetModuleByIdTrackedAsync(Guid id);
}

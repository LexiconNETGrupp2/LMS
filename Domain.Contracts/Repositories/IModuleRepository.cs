using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IModuleRepository : IRepositoryBase<Module>, IInternalRepositoryBase<Module>
{
    Task<IReadOnlyCollection<Module>> GetAllModulesAsync();
    Task<Module?> GetModuleByIdAsync(Guid id);
    Task<IReadOnlyCollection<Module>> GetModulesByCourseIdAsync(Guid courseId);
}

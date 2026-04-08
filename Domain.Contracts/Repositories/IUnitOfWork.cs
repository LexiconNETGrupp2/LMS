using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository Courses { get; }
    IModuleRepository Modules { get; }
    IActivityRepository Activities { get; }
    IUserRepository Users { get; }
    IQueryable<ActivityType> ActivityTypes { get; }

    Task CompleteAsync(CancellationToken token);
}

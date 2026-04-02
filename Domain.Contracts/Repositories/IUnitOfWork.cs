namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository Courses { get; }
    IModuleRepository Modules { get; }
    IActivityRepository Activities { get; }
    IUserRepository Users { get; }

    Task CompleteAsync(CancellationToken token);
}

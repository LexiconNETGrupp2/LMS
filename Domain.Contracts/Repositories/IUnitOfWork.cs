namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository CourseRepository { get; }
    IModuleRepository Modules { get; }
    IActivityRepository Activities { get; }
    IUserRepository Users { get; }

    Task CompleteAsync(CancellationToken token);
}

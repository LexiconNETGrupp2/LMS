namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository CourseRepository { get; }
    IActivityRepository Activities { get; }

    Task CompleteAsync(CancellationToken token);
}
using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;
    private readonly Lazy<ICourseRepository> _courseRepository;

    public ICourseRepository CourseRepository => _courseRepository.Value;
    public IModuleRepository Modules => field ??= new ModuleRepository(context);
    public IActivityRepository Activities => field ??= new ActivityRepository(context);
    public IUserRepository Users => field ??= new UserRepository(context);

    public UnitOfWork(ApplicationDbContext context, Lazy<ICourseRepository> courseRepository)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
    }

    public async Task CompleteAsync(CancellationToken token) => await context.SaveChangesAsync(token);
}

using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;
    
    public ICourseRepository Courses => field ??= new CourseRepository(context);
    public IModuleRepository Modules => field ??= new ModuleRepository(context);
    public IActivityRepository Activities => field ??= new ActivityRepository(context);
    public IUserRepository Users => field ??= new UserRepository(context);

    public UnitOfWork(ApplicationDbContext context, Lazy<ICourseRepository> courseRepository)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task CompleteAsync(CancellationToken token) => await context.SaveChangesAsync(token);
}

using Service.Contracts;

namespace LMS.Services;

public class ServiceManager : IServiceManager
{
    private Lazy<IAuthService> authService;
    private Lazy<ICourseService> courseService;
    private Lazy<IModuleService> moduleService;
    public IAuthService AuthService => authService.Value;
    public ICourseService CourseService => courseService.Value;
    public IModuleService ModuleService => moduleService.Value;


    public ServiceManager(Lazy<IAuthService> authService, Lazy<ICourseService> courseService, Lazy<IModuleService> moduleService)
    {
        this.authService = authService;
        this.courseService = courseService;
        this.moduleService = moduleService;
    }
}

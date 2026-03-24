using Bogus;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace LMS.API.Services;

//You need all this for JWT to work :) 
//User Secrets Json
//Important to have secretkey inside same key "JwtSettings" as used in appsettings.json for get both sections!!!!
//{
//     "password": "YourSecretPasswordHere",
//     "JwtSettings": {
//        "secretkey": "ThisMustBeReallyLong!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
//        }
//}
public class DataSeedHostingService : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IConfiguration configuration;
    private readonly ILogger<DataSeedHostingService> logger;
    private UserManager<ApplicationUser> userManager = null!;
    private RoleManager<IdentityRole> roleManager = null!;
    private const string TeacherRole = "Teacher";
    private const string StudentRole = "Student";

    public DataSeedHostingService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<DataSeedHostingService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment()) return;

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        IReadOnlyCollection<ApplicationUser> users = [];
        if (!(await context.Users.AnyAsync(cancellationToken))) {
            userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));
            ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));

            try {
                await AddRolesAsync([TeacherRole, StudentRole]);
                await AddDemoUsersAsync();
                users = await AddUsersAsync(20);
                logger.LogInformation("Seed complete");
            } catch (Exception ex) {
                logger.LogError($"Data seed fail with error: {ex.Message}");
                throw;
            }
        }

        IReadOnlyCollection<Course> courses = [];
        IReadOnlyCollection<Module> modules = [];
        IReadOnlyCollection<ActivityType> activityTypes = [];
        IReadOnlyCollection<Activity> activities = [];
        IReadOnlyCollection<DocumentType> documentTypes = [];
        IReadOnlyCollection<Document> documents = [];
        if (!(await context.Courses.AnyAsync(cancellationToken))) {
            courses = await AddCoursesToDb(context);
            var course = courses.First();
            course.Students.Add((await userManager.FindByEmailAsync("student@test.com"))!);
            course.Students.Add((await userManager.FindByEmailAsync("teacher@test.com"))!);
            course.Students.AddRange(users);
        }

        if (!(await context.Modules.AnyAsync(cancellationToken))) {
            modules = await AddModulesToCourse(courses.First(), context);
        }

        if (!(await context.ActivityTypes.AnyAsync(cancellationToken))) {
            activityTypes = await AddActivityTypesToDb(context);
        }

        if (!(await context.Activities.AnyAsync(cancellationToken))) {
            activities = await AddActivitiesToModule(modules.First(), activityTypes.First(), context);
        }

        if (!(await context.DocumentTypes.AnyAsync(cancellationToken))) {
            // TODO: Add document types
        }

        if (!(await context.Documents.AnyAsync(cancellationToken))) {
            // TODO: Add documents
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task AddRolesAsync(string[] rolenames)
    {
        foreach (string rolename in rolenames) {
            if (await roleManager.RoleExistsAsync(rolename)) continue;
            var role = new IdentityRole { Name = rolename };
            var res = await roleManager.CreateAsync(role);

            if (!res.Succeeded) throw new Exception(string.Join("\n", res.Errors));
        }
    }
    private async Task AddDemoUsersAsync()
    {
        var teacher = new ApplicationUser {
            UserName = "teacher@test.com",
            Email = "teacher@test.com",
            EmailConfirmed = true,
            FirstName = "Role",
            LastName = "Teacher"
        };

        var student = new ApplicationUser {
            UserName = "student@test.com",
            Email = "student@test.com",
            EmailConfirmed = true,
            FirstName = "Role",
            LastName = "Student"
        };

        await AddUserToDb([teacher, student]);

        var teacherRoleResult = await userManager.AddToRoleAsync(teacher, TeacherRole);
        if (!teacherRoleResult.Succeeded) throw new Exception(string.Join("\n", teacherRoleResult.Errors));

        var studentRoleResult = await userManager.AddToRoleAsync(student, StudentRole);
        if (!studentRoleResult.Succeeded) throw new Exception(string.Join("\n", studentRoleResult.Errors));
    }

    private async Task<IReadOnlyCollection<ApplicationUser>> AddUsersAsync(int nrOfUsers)
    {
        var faker = new Faker<ApplicationUser>("sv").Rules((f, e) =>
        {
            e.FirstName = f.Person.FirstName;
            e.LastName = f.Person.LastName;
            e.Email = f.Person.Email;
            e.UserName = f.Person.Email;
            e.EmailConfirmed = true;
        });

        IReadOnlyCollection<ApplicationUser> users = faker.Generate(nrOfUsers);

        await AddUserToDb(users);
        foreach (var user in users) {
            var roleResult = await userManager.AddToRoleAsync(user, StudentRole);
            if (!roleResult.Succeeded) throw new Exception(string.Join("\n", roleResult.Errors));
        }

        return users;
    }

    private async Task AddUserToDb(IEnumerable<ApplicationUser> users)
    {
        var passWord = configuration["password"];
        ArgumentNullException.ThrowIfNull(passWord, nameof(passWord));

        foreach (var user in users) {
            var result = await userManager.CreateAsync(user, passWord);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        }
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task<IReadOnlyCollection<Course>> AddCoursesToDb(ApplicationDbContext context)
    {
        IReadOnlyCollection<Course> courses = [
            new() {
                Name = "Javascript",
                Description = "Learn JS",
                StartDate = new DateOnly (2025, 10, 16),
                EndDate = new DateOnly(2026, 4, 30),
            },
            new() {
                Name = ".NET",
                Description = "Learn C# .NET",
                StartDate = new DateOnly(2026, 10, 16),
                EndDate = new DateOnly (2027, 2, 10)
            }
        ];

        context.Courses.AddRange(courses);
        return courses;
    }

    private async Task<IReadOnlyCollection<Module>> AddModulesToCourse(Course course, ApplicationDbContext context)
    {
        IReadOnlyCollection<Module> modules = [
            new() {
                Name = "Web API",
                StartDate = new DateOnly(2027, 1, 10), //DateOnly.Parse(new DateTime(2027, 1, 10)),
                EndDate = new DateOnly(2027, 2, 28),
                Course = course,
            },
            new() {
                Name = "Frontend",
                StartDate= new DateOnly (2027, 3, 1),
                EndDate = new DateOnly (2027, 4, 30),
                Course = course
            }
        ];

        context.Modules.AddRange(modules);
        return modules;
    }

    private async Task<IReadOnlyCollection<ActivityType>> AddActivityTypesToDb(ApplicationDbContext context)
    {
        IReadOnlyCollection<ActivityType> types = [
            new() { Name = "Assignment" },
            new() { Name = "Lecture" },
            new() { Name = "E-Learning" },
            new() { Name = "Examination" }
        ];

        context.ActivityTypes.AddRange(types);

        return types;
    }

    private async Task<IReadOnlyCollection<Activity>> AddActivitiesToModule(Module module, ActivityType type, ApplicationDbContext context)
    {
        IReadOnlyCollection<Activity> activities = [
            new() {
                Name = "Lecture 1",
                Type = type,
                StartDate = new DateTime(2025, 10, 17, 9, 0, 0),
                EndDate = new DateTime(2025, 10, 17, 12, 0, 0),
                Module = module
            },
            new() {
                Name = "Assignment 1",
                Type = type,
                StartDate= new DateTime(2025, 10, 17, 12, 0, 0),
                EndDate = new DateTime(2025, 10, 31, 17, 0, 0),
                Module = module
            }
        ];

        context.Activities.AddRange(activities);
        return activities;
    }
}

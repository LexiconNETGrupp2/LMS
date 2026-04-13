using Bogus;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Services;

public sealed class DataSeedHostingService : IHostedService
{
    private const string TeacherRole = "Teacher";
    private const string StudentRole = "Student";

    private const int NumberOfCoursesToSeed = 10;

    private const int MinModulesPerCourse = 4;
    private const int MaxModulesPerCourse = 8;

    private const int MinActivitiesPerModule = 4;
    private const int MaxActivitiesPerModule = 8;

    private const int MinStudentsPerCourse = 10;
    private const int MaxStudentsPerCourse = 15;

    private const int MinTeachersPerCourse = 1;
    private const int MaxTeachersPerCourse = 2;

    private static readonly string[] Roles = [TeacherRole, StudentRole];

    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DataSeedHostingService> _logger;
    private readonly Faker _faker = new("sv");

    private int _generatedUserSequence;

    public DataSeedHostingService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<DataSeedHostingService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment())
            return;

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            await SeedAsync(context, userManager, roleManager, cancellationToken);
            _logger.LogInformation("Seed complete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data seed failed");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        CancellationToken cancellationToken)
    {
        await EnsureRolesAsync(roleManager);
        await EnsureDemoUsersAsync(context, userManager, cancellationToken);

        var courses = await GetOrSeedCoursesAsync(context, cancellationToken);
        await EnsureCourseUsersAsync(context, userManager, courses, cancellationToken);
        await AssignDemoUsersToFirstCourseAsync(userManager, courses, cancellationToken);

        var activityTypes = await GetOrSeedActivityTypesAsync(context, cancellationToken);
        var modules = await GetOrSeedModulesAsync(context, courses, cancellationToken);
        await EnsureActivitiesAsync(context, modules, activityTypes, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in Roles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            EnsureSuccess(result);
        }
    }

    private async Task EnsureDemoUsersAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(cancellationToken))
            return;

        var teacher = new ApplicationUser
        {
            UserName = "teacher@test.com",
            Email = "teacher@test.com",
            EmailConfirmed = true,
            FirstName = "Role",
            LastName = "Teacher"
        };

        var student = new ApplicationUser
        {
            UserName = "student@test.com",
            Email = "student@test.com",
            EmailConfirmed = true,
            FirstName = "Role",
            LastName = "Student"
        };

        await CreateUsersAsync(userManager, [teacher, student]);

        EnsureSuccess(await userManager.AddToRoleAsync(teacher, TeacherRole));
        EnsureSuccess(await userManager.AddToRoleAsync(student, StudentRole));
    }

    private async Task<List<Course>> GetOrSeedCoursesAsync(
    ApplicationDbContext context,
    CancellationToken cancellationToken)
    {
        var existingCourses = await context.Courses.ToListAsync(cancellationToken);
        if (existingCourses.Count > 0)
            return existingCourses;

        var courseFaker = new Faker<Course>("sv")
            .RuleFor(c => c.Name, f => GenerateCourseName(f))
            .RuleFor(c => c.Description, f => f.Lorem.Sentence(18))
            .RuleFor(c => c.StartDate, f => GetRandomDateOnly(
                f,
                new DateOnly(2025, 1, 1),
                new DateOnly(2027, 12, 31)))
            .RuleFor(c => c.EndDate, (f, c) =>
            {
                var monthsToAdd = f.Random.Int(6, 12);
                return c.StartDate.AddMonths(monthsToAdd);
            });

        var courses = courseFaker.Generate(NumberOfCoursesToSeed);

        context.Courses.AddRange(courses);
        return courses;
    }

    private async Task EnsureCourseUsersAsync(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IReadOnlyList<Course> courses,
    CancellationToken cancellationToken)
    {
        var hasCourseUsers = await context.Users.AnyAsync(user => user.CourseId != null, cancellationToken);
        if (hasCourseUsers)
            return;

        foreach (var course in courses)
        {
            var studentCount = _faker.Random.Int(MinStudentsPerCourse, MaxStudentsPerCourse);
            var teacherCount = _faker.Random.Int(MinTeachersPerCourse, MaxTeachersPerCourse);

            await CreateUsersForCourseAsync(userManager, course, studentCount, StudentRole);
            await CreateUsersForCourseAsync(userManager, course, teacherCount, TeacherRole);
        }
    }

    private async Task AssignDemoUsersToFirstCourseAsync(
        UserManager<ApplicationUser> userManager,
        IReadOnlyList<Course> courses,
        CancellationToken cancellationToken)
    {
        if (courses.Count == 0)
            return;

        var firstCourse = courses[0];

        var teacher = await userManager.FindByEmailAsync("teacher@test.com");
        if (teacher is not null && teacher.CourseId is null)
        {
            teacher.Course = firstCourse;
            await userManager.UpdateAsync(teacher);
        }

        var student = await userManager.FindByEmailAsync("student@test.com");
        if (student is not null && student.CourseId is null)
        {
            student.Course = firstCourse;
            await userManager.UpdateAsync(student);
        }
    }

    private async Task<List<ActivityType>> GetOrSeedActivityTypesAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var existingTypes = await context.ActivityTypes.ToListAsync(cancellationToken);
        if (existingTypes.Count > 0)
            return existingTypes;

        var activityTypes = ActivityTypeNames
            .Select(name => new ActivityType { Name = name })
            .ToList();

        context.ActivityTypes.AddRange(activityTypes);
        return activityTypes;
    }

    private async Task<List<Module>> GetOrSeedModulesAsync(
    ApplicationDbContext context,
    IReadOnlyList<Course> courses,
    CancellationToken cancellationToken)
    {
        var existingModules = await context.Modules
            .Include(module => module.Course)
            .ToListAsync(cancellationToken);

        if (existingModules.Count > 0)
            return existingModules;

        var modules = new List<Module>();

        foreach (var course in courses)
        {
            var moduleCount = _faker.Random.Int(MinModulesPerCourse, MaxModulesPerCourse);
            var moduleDateRanges = SplitDateRange(course.StartDate, course.EndDate, moduleCount);

            var moduleFaker = new Faker<Module>("sv")
                .RuleFor(m => m.Name, f => GenerateModuleName(f))
                .RuleFor(m => m.Description, f => f.Lorem.Sentence(16))
                .RuleFor(m => m.Course, _ => course);

            for (var index = 0; index < moduleCount; index++)
            {
                var (startDate, endDate) = moduleDateRanges[index];

                var module = moduleFaker.Generate();
                module.StartDate = startDate;
                module.EndDate = endDate;

                modules.Add(module);
            }
        }

        context.Modules.AddRange(modules);
        return modules;
    }

    private static List<(DateOnly Start, DateOnly End)> SplitDateRange(
    DateOnly start,
    DateOnly end,
    int parts)
    {
        var ranges = new List<(DateOnly Start, DateOnly End)>();

        var totalDays = end.DayNumber - start.DayNumber + 1;
        var baseLength = totalDays / parts;
        var remainder = totalDays % parts;

        var currentStart = start;

        for (var i = 0; i < parts; i++)
        {
            var extraDay = i < remainder ? 1 : 0;
            var length = Math.Max(1, baseLength + extraDay);

            var currentEnd = currentStart.AddDays(length - 1);

            if (currentEnd > end)
                currentEnd = end;

            ranges.Add((currentStart, currentEnd));

            if (i < parts - 1)
                currentStart = currentEnd.AddDays(1);
        }

        return ranges;
    }

    private static string GenerateModuleName(Faker faker)
    {
        var first = Capitalize(faker.Lorem.Word());
        var second = Capitalize(faker.Lorem.Word());

        return $"{first} {second}";
    }

    private static string GenerateActivityName(Faker faker)
    {
        var verb = Capitalize(faker.Hacker.Verb());
        var noun = Capitalize(faker.Hacker.Noun());

        return $"{verb} {noun}";
    }

    private TimeOnly GetRandomStartTimeWithinSchoolHours()
    {
        var validHours = Enumerable.Range(8, 9).ToArray(); // 08:00 - 16:00
        var hour = _faker.PickRandom(validHours);

        var validMinutes = new[] { 0, 15, 30, 45 };
        var minute = _faker.PickRandom(validMinutes);

        return new TimeOnly(hour, minute);
    }

    private static string GenerateCourseName(Faker faker)
    {
        var firstWord = faker.Lorem.Word();
        var secondWord = faker.Lorem.Word();

        return $"{Capitalize(firstWord)} {Capitalize(secondWord)}";
    }

    private static string Capitalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..];
    }

    private static DateOnly GetRandomDateOnly(
        Faker faker,
        DateOnly min,
        DateOnly max)
    {
        var daysBetween = max.DayNumber - min.DayNumber;
        var randomDays = faker.Random.Int(0, daysBetween);
        return min.AddDays(randomDays);
    }

    private async Task EnsureActivitiesAsync(
    ApplicationDbContext context,
    IReadOnlyList<Module> modules,
    IReadOnlyList<ActivityType> activityTypes,
    CancellationToken cancellationToken)
    {
        if (await context.Activities.AnyAsync(cancellationToken))
            return;

        var activities = new List<Activity>();

        foreach (var module in modules)
        {
            var activityCount = _faker.Random.Int(MinActivitiesPerModule, MaxActivitiesPerModule);
            var activityDateRanges = SplitDateRange(module.StartDate, module.EndDate, activityCount);

            for (var index = 0; index < activityCount; index++)
            {
                var (activityDate, _) = activityDateRanges[index];

                var startTime = GetRandomStartTimeWithinSchoolHours();
                var durationHours = _faker.Random.Int(1, 3);
                var endTime = startTime.AddHours(durationHours);

                var latestAllowedEnd = new TimeOnly(17, 0);
                if (endTime > latestAllowedEnd)
                    endTime = latestAllowedEnd;

                if (endTime <= startTime)
                    endTime = startTime.AddHours(1);

                var activityType = _faker.PickRandom(activityTypes.ToArray());

                var activity = new Activity
                {
                    Name = GenerateActivityName(_faker),
                    Description = _faker.Lorem.Sentence(18),
                    Type = activityType,
                    Module = module,
                    StartDate = activityDate.ToDateTime(startTime),
                    EndDate = activityDate.ToDateTime(endTime)
                };

                activities.Add(activity);
            }
        }

        context.Activities.AddRange(activities);
    }

    private async Task CreateUsersForCourseAsync(
    UserManager<ApplicationUser> userManager,
    Course course,
    int count,
    string role)
    {
        var userFaker = new Faker<ApplicationUser>("sv")
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => CreateUniqueEmail(u.FirstName, u.LastName))
            .RuleFor(u => u.UserName, (_, u) => u.Email)
            .RuleFor(u => u.EmailConfirmed, _ => true)
            .RuleFor(u => u.Course, _ => course);

        var users = userFaker.Generate(count);

        await CreateUsersAsync(userManager, users);

        foreach (var user in users)
        {
            var result = await userManager.AddToRoleAsync(user, role);
            EnsureSuccess(result);
        }
    }

    private async Task CreateUsersAsync(
        UserManager<ApplicationUser> userManager,
        IEnumerable<ApplicationUser> users)
    {
        var password = _configuration["password"];
        ArgumentNullException.ThrowIfNull(password);

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, password);
            EnsureSuccess(result);
        }
    }

    private string CreateUniqueEmail(string firstName, string lastName)
    {
        _generatedUserSequence++;

        var normalizedFirstName = NormalizeEmailPart(firstName);
        var normalizedLastName = NormalizeEmailPart(lastName);

        return $"{normalizedFirstName}.{normalizedLastName}{_generatedUserSequence}@example.com";
    }

    private static void EnsureSuccess(IdentityResult result)
    {
        if (result.Succeeded)
            return;

        throw new InvalidOperationException(
            string.Join(Environment.NewLine, result.Errors.Select(error => $"{error.Code}: {error.Description}")));
    }

    private static string NormalizeEmailPart(string value)
    {
        return string.Concat(value
            .ToLowerInvariant()
            .Select(character => character switch
            {
                'å' => "a",
                'ä' => "a",
                'ö' => "o",
                'é' => "e",
                'á' => "a",
                ' ' => "-",
                '.' => string.Empty,
                '\'' => string.Empty,
                _ when char.IsLetterOrDigit(character) || character == '-' => character.ToString(),
                _ => string.Empty
            }));
    }

    private static readonly IReadOnlyList<string> ActivityTypeNames =
    [
        "Inlämning",
        "Övning",
        "Föreläsning",
        "E-Learning",
        "Prov",
        "Examination"
    ];

    private sealed record CourseSeedDefinition(
        string Name,
        string Description,
        DateOnly StartDate,
        DateOnly EndDate,
        IReadOnlyList<ModuleSeedDefinition> Modules);

    private sealed record ModuleSeedDefinition(
        string Name,
        string Description,
        IReadOnlyList<ActivitySeedDefinition> Activities);

    private sealed record ActivitySeedDefinition(
        string Name,
        string Description,
        string ActivityType);
}
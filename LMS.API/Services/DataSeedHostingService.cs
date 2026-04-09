using Bogus;
using LMS.Infractructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    private readonly Faker faker = new("sv");
    private int generatedUserSequence;

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
        userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        ArgumentNullException.ThrowIfNull(roleManager, nameof(roleManager));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));

        try
        {
            await AddRolesAsync([TeacherRole, StudentRole]);

            if (!await context.Users.AnyAsync(cancellationToken))
            {
                await AddDemoUsersAsync();
            }

            List<Course> courses;
            if (!await context.Courses.AnyAsync(cancellationToken))
            {
                courses = AddCoursesToDb(context);

                foreach (var course in courses)
                {
                    await AddStudentsForCourseAsync(course, Random.Shared.Next(10, 16));
                    await AddTeachersForCourseAsync(course, Random.Shared.Next(1, 3));
                }
            }
            else
            {
                courses = await context.Courses.ToListAsync(cancellationToken);
            }

            if (courses.Count > 0)
            {
                await AssignDemoUsersToFirstCourseAsync(courses[0]);
            }

            List<ActivityType> activityTypes;
            if (!await context.ActivityTypes.AnyAsync(cancellationToken))
            {
                activityTypes = AddActivityTypesToDb(context);
            }
            else
            {
                activityTypes = await context.ActivityTypes.ToListAsync(cancellationToken);
            }

            List<Module> modules = [];
            if (!await context.Modules.AnyAsync(cancellationToken))
            {
                modules = AddModulesToCourses(context, courses);
            }
            else
            {
                modules = await context.Modules
                    .Include(module => module.Course)
                    .ToListAsync(cancellationToken);
            }

            if (!await context.Activities.AnyAsync(cancellationToken))
            {
                AddActivitiesToModules(context, modules, activityTypes);
            }

            if (!await context.DocumentTypes.AnyAsync(cancellationToken))
            {
                // TODO: Add document types
            }

            if (!await context.Documents.AnyAsync(cancellationToken))
            {
                // TODO: Add documents
            }

            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seed complete");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Data seed failed");
            throw;
        }
    }

    private async Task AddRolesAsync(string[] rolenames)
    {
        foreach (string rolename in rolenames)
        {
            if (await roleManager.RoleExistsAsync(rolename)) continue;
            var role = new IdentityRole { Name = rolename };
            var res = await roleManager.CreateAsync(role);

            if (!res.Succeeded) throw new Exception(string.Join("\n", res.Errors));
        }
    }

    private async Task AddDemoUsersAsync()
    {
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

        await AddUserToDb([teacher, student]);

        var teacherRoleResult = await userManager.AddToRoleAsync(teacher, TeacherRole);
        if (!teacherRoleResult.Succeeded) throw new Exception(string.Join("\n", teacherRoleResult.Errors));

        var studentRoleResult = await userManager.AddToRoleAsync(student, StudentRole);
        if (!studentRoleResult.Succeeded) throw new Exception(string.Join("\n", studentRoleResult.Errors));
    }

    private async Task AssignDemoUsersToFirstCourseAsync(Course firstCourse)
    {
        var teacher = await userManager.FindByEmailAsync("teacher@test.com");
        if (teacher is not null && teacher.CourseId is null)
        {
            teacher.Course = firstCourse;
        }

        var student = await userManager.FindByEmailAsync("student@test.com");
        if (student is not null && student.CourseId is null)
        {
            student.Course = firstCourse;
        }
    }

    private Task<IReadOnlyCollection<ApplicationUser>> AddStudentsForCourseAsync(Course course, int count)
    {
        return AddUsersForCourseAsync(course, count, StudentRole);
    }

    private Task<IReadOnlyCollection<ApplicationUser>> AddTeachersForCourseAsync(Course course, int count)
    {
        return AddUsersForCourseAsync(course, count, TeacherRole);
    }

    private async Task<IReadOnlyCollection<ApplicationUser>> AddUsersForCourseAsync(Course course, int count, string role)
    {
        var users = Enumerable.Range(0, count)
            .Select(_ => CreateCourseUser(course))
            .ToArray();

        await AddUserToDb(users);

        foreach (var user in users)
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded) throw new Exception(string.Join("\n", roleResult.Errors));
        }

        return users;
    }

    private ApplicationUser CreateCourseUser(Course course)
    {
        generatedUserSequence++;

        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var normalizedFirstName = NormalizeEmailPart(firstName);
        var normalizedLastName = NormalizeEmailPart(lastName);
        var uniqueEmail = $"{normalizedFirstName}.{normalizedLastName}.{generatedUserSequence}@example.com";

        return new ApplicationUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = uniqueEmail,
            UserName = uniqueEmail,
            EmailConfirmed = true,
            Course = course
        };
    }

    private async Task AddUserToDb(IEnumerable<ApplicationUser> users)
    {
        var passWord = configuration["password"];
        ArgumentNullException.ThrowIfNull(passWord, nameof(passWord));

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, passWord);
            if (!result.Succeeded)
                throw new Exception(string.Join("\n", result.Errors.Select(error => $"{error.Code}: {error.Description}")));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private List<Course> AddCoursesToDb(ApplicationDbContext context)
    {
        List<Course> courses =
        [
            new()
            {
                Name = "Databaser och SQL",
                Description = "Kursen ger en introduktion till databashantering och hur man arbetar med relationella databaser. Deltagarna lär sig grunderna i databasspråket SQL och hur man skapar, hämtar och hanterar data i databassystem.",
                StartDate = new DateOnly(2025, 10, 16),
                EndDate = new DateOnly(2026, 4, 30),
            },
            new()
            {
                Name = "C# och .NET-utveckling",
                Description = "Kursen ger en introduktion till programmering i C# och utveckling på .NET-plattformen. Deltagarna lär sig grunderna i objektorienterad programmering och hur man bygger enkla applikationer med moderna utvecklingsverktyg.",
                StartDate = new DateOnly(2026, 10, 16),
                EndDate = new DateOnly(2027, 5, 31)
            },
            new()
            {
                Name = "Frontend-utveckling med React",
                Description = "Kursen introducerar grunderna i modern frontend-utveckling och hur man bygger interaktiva webbgränssnitt. Deltagarna lär sig att skapa komponentbaserade applikationer med React och arbeta med tekniker som JavaScript, HTML och CSS.",
                StartDate = new DateOnly(2026, 1, 5),
                EndDate = new DateOnly(2026, 6, 13)
            },
            new()
            {
                Name = "DevOps och molnutveckling",
                Description = "Kursen introducerar principer och verktyg inom DevOps samt hur moderna applikationer utvecklas och distribueras i molnmiljöer. Deltagarna lär sig grunderna i automatisering, versionshantering och kontinuerlig integration samt arbete med molnplattformar som Microsoft Azure.",
                StartDate = new DateOnly(2026, 2, 1),
                EndDate = new DateOnly(2026, 8, 31)
            },
            new()
            {
                Name = "Systemdesign och arkitektur",
                Description = "Kursen introducerar grundläggande principer för systemdesign och mjukvaruarkitektur. Deltagarna lär sig hur man planerar, strukturerar och dokumenterar skalbara och hållbara systemlösningar med etablerade designprinciper och arkitekturmönster.",
                StartDate = new DateOnly(2026, 9, 1),
                EndDate = new DateOnly(2027, 2, 10)
            },
        ];

        context.Courses.AddRange(courses);
        return courses;
    }

    private List<Module> AddModulesToCourses(ApplicationDbContext context, IEnumerable<Course> courses)
    {
        var modules = new List<Module>();

        foreach (var course in courses)
        {
            var moduleTemplates = GetCourseSeedDefinition(course.Name).Modules;
            var courseLengthDays = course.EndDate.DayNumber - course.StartDate.DayNumber + 1;
            var blockSize = Math.Max(14, courseLengthDays / moduleTemplates.Count);
            var currentStart = course.StartDate;

            for (var index = 0; index < moduleTemplates.Count; index++)
            {
                var template = moduleTemplates[index];
                var isLastModule = index == moduleTemplates.Count - 1;
                var suggestedEnd = currentStart.AddDays(blockSize - 1);
                var moduleEnd = isLastModule || suggestedEnd > course.EndDate ? course.EndDate : suggestedEnd;

                var module = new Module
                {
                    Name = template.Name,
                    Description = template.Description,
                    StartDate = currentStart,
                    EndDate = moduleEnd,
                    Course = course
                };

                modules.Add(module);

                if (!isLastModule)
                {
                    currentStart = moduleEnd.AddDays(1);
                }
            }
        }

        context.Modules.AddRange(modules);
        return modules;
    }

    private List<ActivityType> AddActivityTypesToDb(ApplicationDbContext context)
    {
        List<ActivityType> types =
        [
            new() { Name = "Inlämning" },
            new() { Name = "Övning" },
            new() { Name = "Föreläsning" },
            new() { Name = "E-Learning" },
            new() { Name = "Prov" },
            new() { Name = "Examination" }
        ];

        context.ActivityTypes.AddRange(types);

        return types;
    }

    private List<Activity> AddActivitiesToModules(
        ApplicationDbContext context,
        IEnumerable<Module> modules,
        IEnumerable<ActivityType> activityTypes)
    {
        var activityTypeByName = activityTypes.ToDictionary(type => type.Name, type => type);
        var activities = new List<Activity>();

        foreach (var module in modules)
        {
            var courseDefinition = GetCourseSeedDefinition(module.Course.Name);
            var moduleTemplate = courseDefinition.Modules.First(template => template.Name == module.Name);
            var activityTemplates = moduleTemplate.Activities;
            var totalModuleHours = Math.Max(8, (module.EndDate.DayNumber - module.StartDate.DayNumber + 1) * 8);
            var blockHours = Math.Max(4, totalModuleHours / activityTemplates.Count);
            var currentStart = module.StartDate.ToDateTime(new TimeOnly(8, 0));

            for (var index = 0; index < activityTemplates.Count; index++)
            {
                var template = activityTemplates[index];
                var isLastActivity = index == activityTemplates.Count - 1;
                var suggestedEnd = currentStart.AddHours(blockHours - 1);
                var maxEnd = module.EndDate.ToDateTime(new TimeOnly(17, 0));
                var activityEnd = isLastActivity || suggestedEnd > maxEnd ? maxEnd : suggestedEnd;

                activities.Add(new Activity
                {
                    Name = template.Name,
                    Description = template.Description,
                    Type = activityTypeByName[template.ActivityType],
                    StartDate = currentStart,
                    EndDate = activityEnd,
                    Module = module
                });

                if (!isLastActivity)
                {
                    currentStart = activityEnd.AddHours(1);
                }
            }
        }

        context.Activities.AddRange(activities);
        return activities;
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
                ' ' => "-",
                '.' => string.Empty,
                '\'' => string.Empty,
                _ when char.IsLetterOrDigit(character) || character == '-' => character.ToString(),
                _ => string.Empty
            }));
    }

    private static CourseSeedDefinition GetCourseSeedDefinition(string courseName)
    {
        return courseName switch
        {
            "Databaser och SQL" => new CourseSeedDefinition(
                [
                    new ModuleSeedDefinition(
                        "Databasgrunder",
                        "Introduktion till relationella databaser, tabeller, nycklar och datamodellering.",
                        [
                            new ActivitySeedDefinition("Introduktion till relationsdatabaser", "Genomgång av tabeller, relationer och normalisering.", "Föreläsning"),
                            new ActivitySeedDefinition("ER-modellering workshop", "Praktisk övning i att modellera affärsdata som entiteter och relationer.", "Övning"),
                            new ActivitySeedDefinition("Skapa databasschema", "Skapa tabeller, primärnycklar och främmande nycklar i SQL Server.", "Inlämning"),
                            new ActivitySeedDefinition("Databasdesign quiz", "Kort kunskapskontroll på datamodellering och nyckelbegrepp.", "Prov")
                        ]),
                    new ModuleSeedDefinition(
                        "SQL för datahantering",
                        "Arbete med SELECT, JOIN, filtrering, sortering och aggregering.",
                        [
                            new ActivitySeedDefinition("SELECT och filtrering", "Genomgång av grundläggande SQL-frågor och villkor.", "Föreläsning"),
                            new ActivitySeedDefinition("JOIN-labb", "Övning i att kombinera data från flera tabeller med olika typer av join.", "Övning"),
                            new ActivitySeedDefinition("Rapportfrågor i SQL", "Bygg komplexa frågor med gruppering, sortering och aggregat.", "Inlämning"),
                            new ActivitySeedDefinition("Självrättande SQL-träning", "Interaktiva övningar för att repetera SELECT, WHERE och JOIN.", "E-Learning")
                        ]),
                    new ModuleSeedDefinition(
                        "Avancerad SQL och optimering",
                        "Fördjupning i vyer, transaktioner, indexering och prestanda.",
                        [
                            new ActivitySeedDefinition("Vyer, procedurer och funktioner", "Genomgång av återanvändbara databasobjekt.", "Föreläsning"),
                            new ActivitySeedDefinition("Transaktioner och felhantering", "Övning i ACID, COMMIT, ROLLBACK och säkra uppdateringar.", "Övning"),
                            new ActivitySeedDefinition("Indexering och query-planer", "Analysera prestanda och förbättra långsamma SQL-frågor.", "Inlämning"),
                            new ActivitySeedDefinition("Praktiskt delprov i SQL", "Prov där studenterna löser realistiska databasuppgifter.", "Prov"),
                            new ActivitySeedDefinition("Slutexamination databaser", "Sammanfattande examination av databashantering och SQL.", "Examination")
                        ])
                ]),
            "C# och .NET-utveckling" => new CourseSeedDefinition(
                [
                    new ModuleSeedDefinition(
                        "Introduktion till C#",
                        "Syntax, typer, kontrollflöden och grundläggande felsökning.",
                        [
                            new ActivitySeedDefinition("C# introduktion", "Genomgång av variabler, datatyper och kontrollstrukturer.", "Föreläsning"),
                            new ActivitySeedDefinition("Kodkata i C#", "Parövningar med loopar, villkor och metoder.", "Övning"),
                            new ActivitySeedDefinition("Konsolapplikation med användarinput", "Bygg en mindre konsolapp som hanterar menyval och validering.", "Inlämning"),
                            new ActivitySeedDefinition("Grundläggande C#-syntax", "Digitala övningar för att befästa grunderna i C#.", "E-Learning")
                        ]),
                    new ModuleSeedDefinition(
                        "Objektorienterad programmering",
                        "Klasser, arv, interface, inkapsling och ansvarsfördelning.",
                        [
                            new ActivitySeedDefinition("OOP i praktiken", "Föreläsning om klasser, objekt och SOLID-inspirerat tänk.", "Föreläsning"),
                            new ActivitySeedDefinition("Modellera ett domänproblem", "Övning där studenterna skapar klasser och relationer för ett verkligt scenario.", "Övning"),
                            new ActivitySeedDefinition("Bibliotekssystem i C#", "Implementera ett mindre projekt med klasser, arv och interface.", "Inlämning"),
                            new ActivitySeedDefinition("Kodgranskning OOP", "Gemensam genomgång av kodstruktur och designval.", "Övning")
                        ]),
                    new ModuleSeedDefinition(
                        ".NET och applikationsutveckling",
                        "Arbete med .NET-ekosystemet, beroendeinjektion och enklare API-utveckling.",
                        [
                            new ActivitySeedDefinition(".NET-plattformen och projektstruktur", "Genomgång av solution, projekt, NuGet och beroenden.", "Föreläsning"),
                            new ActivitySeedDefinition("Bygg ett REST-API", "Övning i controllers, endpoints och enkel datahantering.", "Övning"),
                            new ActivitySeedDefinition("Miniapplikation i .NET", "Utveckla en mindre applikation med tydlig lagerindelning.", "Inlämning"),
                            new ActivitySeedDefinition("Kunskapstest .NET", "Prov på centrala koncept inom C# och .NET.", "Prov"),
                            new ActivitySeedDefinition("Praktisk slutexamination", "Examination där studenterna löser en utvecklingsuppgift självständigt.", "Examination")
                        ])
                ]),
            "Frontend-utveckling med React" => new CourseSeedDefinition(
                [
                    new ModuleSeedDefinition(
                        "Webbens byggstenar",
                        "HTML, CSS, JavaScript och hur de samverkar i moderna gränssnitt.",
                        [
                            new ActivitySeedDefinition("HTML och semantik", "Föreläsning om tillgängliga och välstrukturerade webbgränssnitt.", "Föreläsning"),
                            new ActivitySeedDefinition("CSS-layout med Flexbox och Grid", "Praktisk övning i responsiv layout.", "Övning"),
                            new ActivitySeedDefinition("Interaktiv webbkomponent", "Skapa en mindre komponent med JavaScript och DOM-manipulation.", "Inlämning"),
                            new ActivitySeedDefinition("Självstudie i modern CSS", "Digitala moment om responsiv design och komponenttänk.", "E-Learning")
                        ]),
                    new ModuleSeedDefinition(
                        "React och komponenter",
                        "State, props, rendering, formulär och återanvändbara komponenter.",
                        [
                            new ActivitySeedDefinition("React från grunden", "Genomgång av JSX, props, state och komponentstruktur.", "Föreläsning"),
                            new ActivitySeedDefinition("Bygg komponentbibliotek", "Övning i att bryta ned ett UI i återanvändbara komponenter.", "Övning"),
                            new ActivitySeedDefinition("Formulär och validering i React", "Implementera formulär med lokal state och validering.", "Inlämning"),
                            new ActivitySeedDefinition("Kodlabb med hooks", "Praktiska uppgifter med useState och useEffect.", "Övning")
                        ]),
                    new ModuleSeedDefinition(
                        "Frontendapplikationer",
                        "Routing, API-anrop, state-hantering och användarupplevelse.",
                        [
                            new ActivitySeedDefinition("Routing och sidstruktur", "Föreläsning om navigering, layouts och sidflöden i React.", "Föreläsning"),
                            new ActivitySeedDefinition("Hämta data från API", "Övning i fetch, loading states och felhantering.", "Övning"),
                            new ActivitySeedDefinition("Bygg en komplett React-app", "Skapa en mindre applikation med flera vyer och dataflöden.", "Inlämning"),
                            new ActivitySeedDefinition("Frontendtest", "Prov på begrepp och praktiska mönster inom React.", "Prov"),
                            new ActivitySeedDefinition("Slutredovisning frontend", "Examination där projektet presenteras och motiveras.", "Examination")
                        ])
                ]),
            "DevOps och molnutveckling" => new CourseSeedDefinition(
                [
                    new ModuleSeedDefinition(
                        "Versionshantering och arbetsflöden",
                        "Git, branchingstrategier och samarbete i utvecklingsteam.",
                        [
                            new ActivitySeedDefinition("Git och samarbete", "Föreläsning om commits, branches, pull requests och code review.", "Föreläsning"),
                            new ActivitySeedDefinition("Git workshop", "Övning i merge, rebase och konfliktlösning.", "Övning"),
                            new ActivitySeedDefinition("Teamflöde i Git", "Lämna in ett repo med dokumenterat arbetsflöde och branchstrategi.", "Inlämning"),
                            new ActivitySeedDefinition("Digital repetition Git", "Självstudier kring vanliga Git-kommandon och best practices.", "E-Learning")
                        ]),
                    new ModuleSeedDefinition(
                        "CI/CD och automation",
                        "Pipelines, tester, byggsteg och automatiserad leverans.",
                        [
                            new ActivitySeedDefinition("Introduktion till CI/CD", "Föreläsning om pipeline-koncept och kvalitetssäkring.", "Föreläsning"),
                            new ActivitySeedDefinition("Bygg en pipeline", "Övning i att sätta upp bygg- och teststeg för en applikation.", "Övning"),
                            new ActivitySeedDefinition("Automatiserad deployment", "Skapa en pipeline som publicerar en applikation till testmiljö.", "Inlämning"),
                            new ActivitySeedDefinition("Pipeline review", "Gemensam genomgång av pipeline-design och förbättringar.", "Övning")
                        ]),
                    new ModuleSeedDefinition(
                        "Molntjänster och drift",
                        "Grundläggande Azure-kunskap, containerisering och övervakning.",
                        [
                            new ActivitySeedDefinition("Molnarkitektur i Azure", "Föreläsning om compute, storage, identitet och ansvarsfördelning.", "Föreläsning"),
                            new ActivitySeedDefinition("Containerisering med Docker", "Praktisk övning i att paketera applikationer i containers.", "Övning"),
                            new ActivitySeedDefinition("Distribuera till molnet", "Implementera en deployment till molnmiljö med enkel övervakning.", "Inlämning"),
                            new ActivitySeedDefinition("Drift och observability", "Prov på centrala DevOps- och molnbegrepp.", "Prov"),
                            new ActivitySeedDefinition("Slutexamination DevOps", "Examination där en komplett leveranskedja demonstreras.", "Examination")
                        ])
                ]),
            "Systemdesign och arkitektur" => new CourseSeedDefinition(
                [
                    new ModuleSeedDefinition(
                        "Arkitekturprinciper",
                        "Introduktion till lagerindelning, ansvar, koppling och cohesion.",
                        [
                            new ActivitySeedDefinition("Grunder i mjukvaruarkitektur", "Föreläsning om arkitekturstilar, trade-offs och kvalitetsattribut.", "Föreläsning"),
                            new ActivitySeedDefinition("Analysera ett system", "Övning där studenterna identifierar ansvar och beroenden i en befintlig lösning.", "Övning"),
                            new ActivitySeedDefinition("Arkitekturskiss", "Skapa en enkel systemskiss med lager, komponenter och ansvar.", "Inlämning"),
                            new ActivitySeedDefinition("Begreppsträning arkitektur", "E-learning med fokus på vanliga principer och mönster.", "E-Learning")
                        ]),
                    new ModuleSeedDefinition(
                        "Designmönster och integration",
                        "Arbete med designmönster, kommunikation mellan system och gränssnitt.",
                        [
                            new ActivitySeedDefinition("Designmönster i praktiken", "Föreläsning om vanliga patterns och när de passar.", "Föreläsning"),
                            new ActivitySeedDefinition("Integrationsövning", "Praktisk övning i att modellera API-kontrakt och externa beroenden.", "Övning"),
                            new ActivitySeedDefinition("Komponentdesign", "Designa en lösning med tydliga kontrakt och mönsteranvändning.", "Inlämning"),
                            new ActivitySeedDefinition("Arkitekturworkshop", "Peer review av komponentindelning och integrationsval.", "Övning")
                        ]),
                    new ModuleSeedDefinition(
                        "Skalbarhet och dokumentation",
                        "Kvalitetsattribut, risker, dokumentation och tekniska beslut.",
                        [
                            new ActivitySeedDefinition("Skalbarhet och robusthet", "Föreläsning om prestanda, tillgänglighet och driftsäkerhet.", "Föreläsning"),
                            new ActivitySeedDefinition("ADR och tekniska beslut", "Övning i att dokumentera arkitekturbeslut och konsekvenser.", "Övning"),
                            new ActivitySeedDefinition("Arkitekturdokument", "Ta fram ett underlag med målbild, risker och föreslagen lösning.", "Inlämning"),
                            new ActivitySeedDefinition("Systemdesignprov", "Prov på kvalitetsattribut, mönster och arkitekturella val.", "Prov"),
                            new ActivitySeedDefinition("Slutpresentation arkitektur", "Examination där hela systemdesignen presenteras och försvaras.", "Examination")
                        ])
                ]),
            _ => throw new InvalidOperationException($"Missing course seed definition for course '{courseName}'.")
        };
    }

    private sealed record CourseSeedDefinition(IReadOnlyList<ModuleSeedDefinition> Modules);

    private sealed record ModuleSeedDefinition(
        string Name,
        string Description,
        IReadOnlyList<ActivitySeedDefinition> Activities);

    private sealed record ActivitySeedDefinition(
        string Name,
        string Description,
        string ActivityType);
}

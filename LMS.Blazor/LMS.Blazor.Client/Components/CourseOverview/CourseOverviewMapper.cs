using LMS.Shared.DTOs.CourseDtos;

namespace LMS.Blazor.Client.Components.CourseOverview;

public static class CourseOverviewMapper
{
    public static CourseOverviewViewModel Map(CourseDto result)
    {
        return new CourseOverviewViewModel {
            Title = result.Name,
            Description = result.Description,
            DateRange = $"{result.StartDate} - {result.EndDate}",
            ProgressPercent = Random.Shared.Next(0, 100), // TODO: Calculate progress percent
            Modules = result.Modules.Select(s => new ModuleViewModel()
            {
                Id = s.Id.ToString(),
                Title = s.Name,
                DateRange = $"{s.StartDate} - {s.EndDate}",
                // Expand when current date is between start and end date of the module
                IsExpanded = DateTime.Now >= s.StartDate.ToDateTime(TimeOnly.MinValue) &&
                             DateTime.Now <= s.EndDate.ToDateTime(TimeOnly.MaxValue),
                ProgressPercent = Random.Shared.Next(0, 100), // TODO: Calculate progress percent based on activities
                Activities = s.Activities.Select(a => new ActivityViewModel()
                {
                    Title = a.Name,
                    Type = a.Type.Name,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                }).ToArray(),
            }).ToArray(),
        };
    }
}

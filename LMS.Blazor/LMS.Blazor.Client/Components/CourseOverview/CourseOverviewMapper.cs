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
            ProgressPercent = 0,
            Modules = result.Modules.Select(s => new ModuleViewModel()
            {
                Id = s.Id.ToString(),
                Title = s.Name,
                DateRange = $"{s.StartDate} - {s.EndDate}",
                // Expand when current date is between start and end date of the module
                IsExpanded = DateTime.Now >= s.StartDate.ToDateTime(TimeOnly.MinValue) &&
                             DateTime.Now <= s.EndDate.ToDateTime(TimeOnly.MaxValue),
                Activities = s.Activities.Select(a => new ActivityViewModel()
                {
                    Title = a.Name,
                    Type = a.Type.Name,
                    Date = $"{a.StartDate} - {a.EndDate}",
                }).ToArray(),
            }).ToArray(),
        };
    }
}

using LMS.Blazor.Client.Components.Modules;

namespace LMS.Blazor.Client.Components.CreateCourse;

public class CourseFormModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<ModuleFormModel> Modules { get; set; } = [];
}

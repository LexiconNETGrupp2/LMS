namespace LMS.Blazor.Client.Components.TeacherCourseOverview;

public class ModuleProgressModel
{
    public string Name { get; set; } = "";
    public int Progress { get; set; }
}

public class CourseStatisticsModel
{
    public int Modules { get; set; }
    public int Activities { get; set; }
    public int Students { get; set; }
    public int Submissions { get; set; }
}

public class ActivityModel
{
    public string Title { get; set; } = "";
    public string Time { get; set; } = "";
    public ActivityType Type { get; set; }
}

public class ModuleModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Progress { get; set; }
    public bool IsExpanded { get; set; }
    public List<ModuleActivityModel> Activities { get; set; } = [];
}

public class ModuleActivityModel
{
    public string Title { get; set; } = "";
    public ModuleActivityType Type { get; set; }
    public DateTime Date { get; set; }
    public DateTime? Deadline { get; set; }
    public bool ShowSubmissionButton { get; set; }
    public List<ModuleDocumentModel> Documents { get; set; } = [];
}

public class ModuleDocumentModel
{
    public string FileName { get; set; } = "";
    public string FileSize { get; set; } = "";
}
public class StudentListItemModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ProgressPercent { get; set; }
    public DateTime LastActive { get; set; }
}

public enum ModuleActivityType
{
    Lecture,
    Exercise,
    Assignment
}

public enum ActivityType
{
    Submission,
    Document,
    Module
}
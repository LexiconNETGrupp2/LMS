namespace Domain.Models.Entities;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required DocumentType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    //public string UploaderId { get; set; } = string.Empty;
    //public Guid? CourseId { get; set; } = Guid.NewGuid();
    //public Guid? ModuleId { get; set; } = Guid.NewGuid();
    //public Guid? ActivityId { get; set; } = Guid.NewGuid();
    // Navigational
    public required ApplicationUser Uploader { get; set; }
    public Course? Course { get; set; }
    public Module? Module { get; set; }
    public Activity? Activity { get; set; }
}

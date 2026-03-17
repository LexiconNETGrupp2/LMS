namespace Domain.Models.Entities;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<ApplicationUser> Students { get; set; } = [];
    public ICollection<Module> Modules { get; set; } = [];
}

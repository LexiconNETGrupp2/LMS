namespace Domain.Models.Entities;

public class Module
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required Course Course { get; set; }
    public ICollection<Activity> Activities { get; set; } = [];
}

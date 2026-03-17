namespace Domain.Models.Entities;

public class Activity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required ActivityType Type { get; set; }
    public required Module Module { get; set; }
}

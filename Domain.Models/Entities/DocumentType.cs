namespace Domain.Models.Entities;

public class DocumentType
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Document> Documents { get; set; } = [];
}

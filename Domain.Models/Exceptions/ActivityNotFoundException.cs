namespace Domain.Models.Exceptions;

public class ActivityNotFoundException : NotFoundException
{
    public ActivityNotFoundException() 
        : base("Module not found")
    { }

    public ActivityNotFoundException(Guid id)
        : base($"No module with id '{id}' was found")
    { }
}

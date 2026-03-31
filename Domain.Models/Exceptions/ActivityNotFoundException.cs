namespace Domain.Models.Exceptions;

public class ActivityNotFoundException : NotFoundException
{
    public ActivityNotFoundException() : base("Activity not found") { }

    public ActivityNotFoundException(Guid id) : base($"No activity with id '{id}' was found") { }
}

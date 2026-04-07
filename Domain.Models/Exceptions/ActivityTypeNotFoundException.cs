namespace Domain.Models.Exceptions;

public class ActivityTypeNotFoundException(string typeName)
    : NotFoundException("Activity type not found: " + typeName);

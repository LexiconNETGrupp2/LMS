namespace Domain.Models.Exceptions;

public class ModuleNotFoundException : NotFoundException
{
    public ModuleNotFoundException() 
        : base("Module not found")
    { }

    public ModuleNotFoundException(Guid id)
        : base($"No module with id '{id}' was found")
    { }
}

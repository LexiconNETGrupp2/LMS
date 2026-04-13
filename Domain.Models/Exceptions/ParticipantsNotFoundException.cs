namespace Domain.Models.Exceptions;

public class ParticipantsNotFoundException : NotFoundException
{
    public ParticipantsNotFoundException() : base("No participants were found") { }
    public ParticipantsNotFoundException(string message) : base(message) { }
}

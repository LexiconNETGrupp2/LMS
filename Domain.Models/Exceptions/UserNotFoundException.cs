namespace Domain.Models.Exceptions;

public class UserNotFoundException()
    : NotFoundException("User not found");

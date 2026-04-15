namespace Domain.Models.Exceptions;

public class UserForbiddenException(string message ="You're not allowed to access this resource", int code = 403) 
    : TokenValidationException(message, code)
{
    public UserForbiddenException() 
        : this("You're not allowed to access this resource", 403) { }
}

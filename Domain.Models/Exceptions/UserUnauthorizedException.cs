namespace Domain.Models.Exceptions;

public class UserUnauthorizedException : TokenValidationException
{
    public UserUnauthorizedException() 
        : base("Not allowed to access this resource") 
    { }
}

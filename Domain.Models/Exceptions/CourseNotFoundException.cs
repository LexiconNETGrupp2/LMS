namespace Domain.Models.Exceptions;

public class CourseNotFoundException : NotFoundException
{
    public CourseNotFoundException() : base("Course not found") { }
    public CourseNotFoundException(Guid id) : base($"Course {id} not found") { }
}

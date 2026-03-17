using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Course>> GetAllCourses()
    {
        return await _context.Courses
                        .AsNoTracking()
                        .ToListAsync();
    }

    public async Task<Course?> GetCourseFromUserId(Guid UserId)
    {
        string userIdStr = UserId.ToString();
        return await _context.Courses
                        .AsNoTracking()
                        .Where(c => c.Students.FirstOrDefault(u => u.Id == userIdStr) != null)
                        .FirstOrDefaultAsync();
    }
}

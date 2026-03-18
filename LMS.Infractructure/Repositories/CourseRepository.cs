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
                        .Include(c => c.Modules)
                        .Include(c => c.Students)
                        .ToListAsync();
    }

    public async Task<Course?> GetCourseById(Guid id)
    {
        return await _context.Courses
                        .AsNoTracking()
                        .Include(c => c.Modules)
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyCollection<Course>> GetCourseFromUserId(Guid userId)
    {
        string userIdStr = userId.ToString();
        return await _context.Courses
                        .AsNoTracking()
                        .Where(c => c.Students.FirstOrDefault(u => u.Id == userIdStr) != null)
                        .Include(c => c.Modules)
                        .Include(c => c.Students)
                        .ToListAsync();
    }
}

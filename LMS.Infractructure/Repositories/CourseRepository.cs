using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.Constants;
using LMS.Shared.DTOs.CourseDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Course>> GetAllCourses(AllCoursesParams param, CancellationToken token)
    {
        var query = FindAll(trackChanges: false);

        if (param.AfterDate is not null) {
            query = query.Where(c => c.StartDate <= param.AfterDate);
        }

        if (param.BeforeDate is not null) {
            query = query.Where(c => c.EndDate >= param.BeforeDate);
        }

        if (param.Search is not null) {
            query = query.Where(c => c.Name.Contains(param.Search) || 
                                c.Description.Contains(param.Search));
        }

        return await query.Include(c => c.Modules)
                        .Include(c => c.Students)
                        .ToListAsync(token);
    }

    public async Task<Course?> GetCourseById(Guid id, bool trackChanges, CancellationToken token)
    {
        return await FindAll(trackChanges: trackChanges)
                        .Include(c => c.Modules)
                            .ThenInclude(m => m.Activities)
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(c => c.Id == id, token);
    }

    public async Task<Course?> GetCourseFromUserId(Guid userId, bool trackChanges, CancellationToken token)
    {
        var userIdStr = userId.ToString();

        return await FindAll(trackChanges: trackChanges)
                        .AsNoTracking()
                        .Where(c => c.Students.FirstOrDefault(u => u.Id == userIdStr) != null)
                        .Include(c => c.Modules)
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(token);
    }
}

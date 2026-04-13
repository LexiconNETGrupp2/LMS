using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Infractructure.Extensions;
using LMS.Shared.Constants;
using LMS.Shared.DTOs.CourseDtos;
using LMS.Shared.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class CourseRepository(ApplicationDbContext context)
    : RepositoryBase<Course>(context), ICourseRepository
{
    public async Task<PagedResult<Course>> GetAllCourses(AllCoursesParams param, CancellationToken token)
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

        return await query
                        .Include(c => c.Modules)
                            .ThenInclude(m => m.Activities)
                                .ThenInclude(a => a.Type)
                        .Include(c => c.Students)
                        .OrderBy(c => c.StartDate)
                        .ToPagedResultAsync(param, token);
    }

    public async Task<Course?> GetCourseById(Guid id, bool trackChanges, CancellationToken token, bool includeAllData = true)
    {
        var query = FindAll(trackChanges: trackChanges);
        if (includeAllData) {
            query = query.Include(c => c.Modules)
                            .ThenInclude(m => m.Activities)
                                .ThenInclude(a => a.Type)
                          .Include(c => c.Students);
        }
        return await query.FirstOrDefaultAsync(c => c.Id == id, token);
    }

    public async Task<Course?> GetCourseFromUserId(Guid userId, bool trackChanges, CancellationToken token)
    {
        var userIdStr = userId.ToString();

        return await FindAll(trackChanges: trackChanges)
                        .AsNoTracking()
                        .Where(c => c.Students.FirstOrDefault(u => u.Id == userIdStr) != null)
                        .Include(c => c.Modules.OrderBy(m => m.StartDate))
                            .ThenInclude(m => m.Activities)
                                .ThenInclude(a => a.Type)
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(token);
    }
}

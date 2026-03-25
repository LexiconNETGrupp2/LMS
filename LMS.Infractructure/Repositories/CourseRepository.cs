using Domain.Contracts.Repositories;
using Domain.Contracts.Repositories.Models;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
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
        var query = _context.Courses
                        .AsNoTracking()
                        .AsQueryable();

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

    public async Task<Course?> GetCourseById(Guid id, CancellationToken token)
    {
        return await _context.Courses
                        .AsNoTracking()
                        .Include(c => c.Modules)
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(c => c.Id == id, token);
    }

    public async Task<Course?> GetCourseByIdTracked(Guid id, CancellationToken token)
    {
        return await _context.Courses
                        .Include(c => c.Modules)
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(c => c.Id == id, token);
    }

    public async Task<Course?> GetCourseFromUserId(Guid userId, CancellationToken token)
    {
        var userIdStr = userId.ToString();

        return await _context.Courses
                        .AsNoTracking()
                        .Where(c => c.Students.FirstOrDefault(u => u.Id == userIdStr) != null)
                        .Include(c => c.Modules)
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(token);
    }

    public async Task<CourseParticipantsReadModel?> GetCourseParticipantsByUserId(Guid userId, CancellationToken token)
    {
        var userIdStr = userId.ToString();

        var courseData = await _context.Courses
                        .AsNoTracking()
                        .Where(c => c.Students.Any(u => u.Id == userIdStr))
                        .Include(c => c.Students)
                        .FirstOrDefaultAsync(token);

        if (courseData is null)
            return null;

        var participants = courseData.Students
                           .Select(student => new CourseParticipantReadModel
                           {
                               Id = student.Id,
                               FirstName = student.FirstName.Trim(),
                               LastName = student.LastName.Trim(),
                               Email = student.Email ?? string.Empty
                           })
                           .ToList();

        var participantIds = participants.Select(participant => participant.Id).ToList();
        var participantRoles = await GetParticipantRolesAsync(participantIds, token);

        return new CourseParticipantsReadModel
        {
            CourseId = courseData.Id,
            Name = courseData.Name,
            Description = courseData.Description,
            Participants = participants,
            ParticipantRoles = participantRoles
        };
    }

    private async Task<IReadOnlyCollection<CourseParticipantRoleReadModel>> GetParticipantRolesAsync(
        IReadOnlyCollection<string> participantIds,
        CancellationToken token)
    {
        if (participantIds.Count == 0)
            return [];

        return await (
                        from userRole in _context.Set<IdentityUserRole<string>>().AsNoTracking()
                        join role in _context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                        where participantIds.Contains(userRole.UserId)
                        select new CourseParticipantRoleReadModel
                        {
                            UserId = userRole.UserId,
                            RoleName = role.Name
                        })
                        .ToListAsync(token);
    }
}

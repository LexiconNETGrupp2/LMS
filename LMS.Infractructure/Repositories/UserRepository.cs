using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Infractructure.Extensions;
using LMS.Shared.DTOs.UserDtos;
using LMS.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class UserRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationUser>(context), IUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PagedResult<ApplicationUser>> GetAllWithCoursesAsync(AllUsersParams query, CancellationToken ct)
    {
        var users = FindAll();
        
        if (query.Role is not null) {
            users = from user in users
                    join userRole in _context.UserRoles.AsNoTracking()
                        on user.Id equals userRole.UserId
                    join role in _context.Roles.AsNoTracking()
                         on userRole.RoleId equals role.Id
                    where role.Name == query.Role
                    select user;
        }

        if (query.Search is not null) {
            users = users.Where(u => u.FirstName.Contains(query.Search)
                                  || u.LastName.Contains(query.Search)
                                  || u.Email!.Contains(query.Search));
        }

        string orderBy = query.OrderBy is not null ? query.OrderBy : "FullName";
        bool isDesc = query.IsDescending.Value && true;
        users = query.OrderBy switch {
            "FullName" => isDesc ? 
                        users.OrderByDescending(u => u.FirstName).ThenBy(u => u.LastName) : 
                        users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
            "Email" => isDesc ?
                        users.OrderByDescending(u => u.Email) :
                        users.OrderBy(u => u.Email),
            "CourseName" => isDesc ?
                        users.OrderByDescending(u => u.Course.Name) :
                        users.OrderBy(u => u.Course.Name),
            _ => users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
        };

        return await users.Include(u => u.Course)
                     .ToPagedResultAsync(query, ct);
    }

    public async Task<ApplicationUser?> GetByIdWithCourseAsync(string id, CancellationToken ct)
    {
        return await FindByCondition(u => u.Id == id)
            .Include(u => u.Course)
            .SingleOrDefaultAsync(ct);
    }
}

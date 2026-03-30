using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class UserRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationUser>(context), IUserRepository
{

    public async Task<IReadOnlyCollection<ApplicationUser>> GetAllWithCoursesAsync(CancellationToken ct)
    {
        return await FindAll()
            .Include(u => u.Course)
            .ToListAsync(ct);
    }

    public async Task<ApplicationUser?> GetByIdWithCourseAsync(string id, CancellationToken ct)
    {
        return await FindByCondition(u => u.Id == id)
            .Include(u => u.Course)
            .SingleOrDefaultAsync(ct);
    }
}

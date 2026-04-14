using LMS.Shared.DTOs.CourseDtos;

namespace Domain.Contracts.Queries;

public interface IParticipantQuery
{
    Task<CourseParticipantsDto?> GetCourseParticipantsByUserId(Guid userId, CancellationToken token);
    Task<IReadOnlyCollection<CourseStudentDto>> GetStudentsByCourseId(Guid courseId, CancellationToken token);
    Task<IReadOnlyDictionary<Guid, CourseParticipantCounts>> GetCourseParticipantCountsByCourseIds(
        IReadOnlyCollection<Guid> courseIds,
        CancellationToken token);
}

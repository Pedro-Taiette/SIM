using School.Domain.Entities;

namespace School.Domain.Abstractions;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetAsync(Guid studentId, Guid courseId);
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Enrollment>> GetByCourseIdAsync(Guid courseId);
    Task AddAsync(Enrollment enrollment);
    Task RemoveAsync(Enrollment enrollment);
}

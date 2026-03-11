namespace School.Domain.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IStudentRepository Students { get; }
    ICourseRepository Courses { get; }
    IEnrollmentRepository Enrollments { get; }

    Task<bool> CommitAsync();
}

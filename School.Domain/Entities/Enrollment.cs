using School.Domain.Abstractions;
using School.Domain.Constants;
using School.Domain.Exceptions;

namespace School.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }

    public Student Student { get; private set; } = null!;
    public Course Course { get; private set; } = null!;

    private Enrollment() { }

    public Enrollment(Guid studentId, Guid courseId)
    {
        if (studentId == Guid.Empty)
            throw new DomainValidationException(ValidationMessages.InvalidStudentId);

        if (courseId == Guid.Empty)
            throw new DomainValidationException(ValidationMessages.InvalidCourseId);

        StudentId = studentId;
        CourseId = courseId;
    }
}

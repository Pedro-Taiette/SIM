using School.Domain.Abstractions;
using School.Domain.Constants;
using School.Domain.Exceptions;

namespace School.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; private set; }
    public int WorkloadHours { get; private set; }
    public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

    public Course(string title, int workloadHours)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainValidationException(ValidationMessages.CourseTitleRequired);

        if (workloadHours < CourseConstants.MinWorkloadHours || workloadHours > CourseConstants.MaxWorkloadHours)
            throw new DomainValidationException(ValidationMessages.InvalidWorkload);

        Title = title;
        WorkloadHours = workloadHours;
    }

    public ICollection<Student> Students { get; private set; } = new List<Student>();
}

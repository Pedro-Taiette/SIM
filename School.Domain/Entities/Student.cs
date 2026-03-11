using School.Domain.Abstractions;
using School.Domain.Constants;
using School.Domain.Exceptions;

namespace School.Domain.Entities;

public class Student : BaseEntity
{
    public string FirstName { get; private set; }
    public string Email { get; private set; }
    public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

    public Student(string firstName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > 50)
            throw new DomainValidationException(ValidationMessages.StudentNameRequired);

        if (!email.EndsWith("@faculdade.edu", StringComparison.OrdinalIgnoreCase))
            throw new DomainValidationException(ValidationMessages.InvalidEmailDomain);

        FirstName = firstName;
        Email = email;
    }

    public ICollection<Course> Courses { get; private set; } = new List<Course>();
}
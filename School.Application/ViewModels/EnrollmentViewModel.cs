namespace School.Application.ViewModels;

public record EnrollmentViewModel(Guid Id, Guid StudentId, Guid CourseId, DateTime CreatedAt);

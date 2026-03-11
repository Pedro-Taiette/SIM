using School.Application.Abstractions;
using School.Application.Exceptions;
using School.Application.ViewModels;
using School.Domain.Abstractions;
using School.Domain.Constants;
using School.Domain.Entities;
using School.Domain.Exceptions;

namespace School.Application.Services;

public class EnrollmentService: IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task EnrollStudentAsync(Guid studentId, Guid courseId)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(studentId)
            ?? throw new BusinessLogicException(ValidationMessages.StudentNotFound);

        var course = await _unitOfWork.Courses.GetByIdAsync(courseId)
            ?? throw new BusinessLogicException(ValidationMessages.CourseNotFound);

        var existingEnrollment = await _unitOfWork.Enrollments.GetAsync(studentId, courseId);
        if (existingEnrollment != null)
            throw new BusinessLogicException(ValidationMessages.EnrollmentDuplicate);

        int currentTotal = student.Enrollments.Sum(e => e.Course.WorkloadHours);

        if (currentTotal + course.WorkloadHours > CourseConstants.MaxTotalSemesterWorkload)
            throw new DomainValidationException(ValidationMessages.EnrollmentLimitExceeded);

        var enrollment = new Enrollment(studentId, courseId);
        await _unitOfWork.Enrollments.AddAsync(enrollment);
        await _unitOfWork.CommitAsync();
    }

    public async Task UnenrollAsync(Guid enrollmentId)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId)
            ?? throw new BusinessLogicException(ValidationMessages.EnrollmentNotFound);

        await _unitOfWork.Enrollments.RemoveAsync(enrollment);
        await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<EnrollmentViewModel>> GetByStudentAsync(Guid studentId)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(studentId)
            ?? throw new BusinessLogicException(ValidationMessages.StudentNotFound);

        var enrollments = await _unitOfWork.Enrollments.GetByStudentIdAsync(studentId);
        return enrollments.Select(e => new EnrollmentViewModel(e.Id, e.StudentId, e.CourseId, e.CreatedAt));
    }

    public async Task<IEnumerable<EnrollmentViewModel>> GetByCourseAsync(Guid courseId)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId)
            ?? throw new BusinessLogicException(ValidationMessages.CourseNotFound);

        var enrollments = await _unitOfWork.Enrollments.GetByCourseIdAsync(courseId);
        return enrollments.Select(e => new EnrollmentViewModel(e.Id, e.StudentId, e.CourseId, e.CreatedAt));
    }
}

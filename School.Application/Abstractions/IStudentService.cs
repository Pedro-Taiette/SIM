using School.Application.ViewModels;

namespace School.Application.Abstractions;

public interface IStudentService
{
    Task EnrollAsync(CreateStudentViewModel viewModel);
    Task<IEnumerable<StudentViewModel>> GetAllAsync();
    Task DeactivateAsync(Guid id);
}
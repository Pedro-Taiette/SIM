using School.Application.ViewModels;

namespace School.Application.Abstractions;

public interface ICourseService
{
    Task<CourseViewModel> CreateAsync(CreateCourseViewModel viewModel);
    Task<IEnumerable<CourseViewModel>> GetAllAsync();
    Task DeleteAsync(Guid id);
}
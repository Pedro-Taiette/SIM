using School.Domain.Entities;

namespace School.Domain.Abstractions;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<Student?> GetByEmailAsync(string email);
    Task<IEnumerable<Student>> GetAllAsync();
    Task AddAsync(Student student);
}

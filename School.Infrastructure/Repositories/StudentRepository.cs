using School.Domain.Abstractions;
using School.Domain.Entities;
using School.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace School.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly SchoolDbContext _context;

    public StudentRepository(SchoolDbContext context) => _context = context;

    public async Task AddAsync(Student student) => await _context.Students.AddAsync(student);

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
        => await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .ToListAsync();

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}

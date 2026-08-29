using estudiantes_icinf.Models;

namespace estudiantes_icinf.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(Guid id);
    Task<Student?> GetByEmailAsync(string email, Guid? excludeId = null);
    Task<Student> AddAsync(CreateStudentDto dto);
    Task<Student?> UpdateAsync(Guid id, UpdateStudentDto dto);
    Task DeleteAsync(Guid id);
}

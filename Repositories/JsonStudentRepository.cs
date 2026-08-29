using System.Text.Json;
using estudiantes_icinf.Models;

namespace estudiantes_icinf.Repositories;

public class JsonStudentRepository : IStudentRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonStudentRepository(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "students.json");
    }

    public async Task<List<Student>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return await ReadAllAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        var students = await GetAllAsync();
        return students.FirstOrDefault(s => s.Id == id);
    }

    public async Task<Student?> GetByEmailAsync(string email, Guid? excludeId = null)
    {
        var students = await GetAllAsync();
        return students.FirstOrDefault(s =>
            string.Equals(s.Email, email, StringComparison.OrdinalIgnoreCase) && s.Id != excludeId);
    }

    public async Task<Student> AddAsync(CreateStudentDto dto)
    {
        await _lock.WaitAsync();
        try
        {
            var students = await ReadAllAsync();
            var ahora = DateTime.UtcNow;
            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Age = dto.Age,
                CreatedAt = ahora,
                UpdatedAt = ahora
            };
            students.Add(student);
            await WriteAllAsync(students);
            return student;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Student?> UpdateAsync(Guid id, UpdateStudentDto dto)
    {
        await _lock.WaitAsync();
        try
        {
            var students = await ReadAllAsync();
            var student = students.FirstOrDefault(s => s.Id == id);
            if (student is null)
            {
                return null;
            }

            if (dto.Name is not null) student.Name = dto.Name;
            if (dto.Email is not null) student.Email = dto.Email;
            if (dto.Age is not null) student.Age = dto.Age.Value;
            student.UpdatedAt = DateTime.UtcNow;

            await WriteAllAsync(students);
            return student;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await _lock.WaitAsync();
        try
        {
            var students = await ReadAllAsync();
            students.RemoveAll(s => s.Id == id);
            await WriteAllAsync(students);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<Student>> ReadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        var students = await JsonSerializer.DeserializeAsync<List<Student>>(stream, SerializerOptions);
        return students ?? [];
    }

    private async Task WriteAllAsync(List<Student> students)
    {
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, students, SerializerOptions);
    }
}

using Database.DTO.User;
using Database.Models;

namespace Database.Interfaces;

public interface IUserRepository
{
    Task<User?> GetStudentAsync(Guid id);
    Task<User?> GetTeacherAsync(Guid id);
    Task<User?> GetUserAsync(Guid id);
    Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id);
    bool UserExists(Guid id);
    bool UserExists(int primaryKey);
    Task AddStudentAsync(UserAdd student);
    Task AddTeacherAsync(UserAdd teacher);
}

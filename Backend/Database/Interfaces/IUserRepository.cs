using Database.DTO.User;

namespace Database.Interfaces;

public interface IUserRepository
{
    Task<FullUser?> GetStudentAsync(Guid id);
    Task<FullUser?> GetTeacherAsync(Guid id);
    Task<FullUser?> GetUserAsync(Guid id);
    Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id);
    bool UserExists(Guid id);
    bool UserExists(int primaryKey);
    Task AddStudentAsync(UserAdd student);
    Task AddTeacherAsync(UserAdd teacher);
}

using Database.DTO.ActiveQuestionnaire;
using Database.DTO.User;
using Database.Models;

namespace Database.Interfaces;

public interface IUserRepository
{
    Task<FullUser?> GetStudentAsync(Guid id);
    Task<FullUser?> GetTeacherAsync(Guid id);
    Task<FullUser?> GetUserAsync(Guid id);
    Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id);
    Task<List<UserSpecificActiveQuestionnaireBase>> GetAllAssociatedActiveQuestionnaires(Guid userId);
    bool UserExists(Guid id);
    bool UserExists(int primaryKey);
    Task AddStudentAsync(UserAdd student);
    Task AddTeacherAsync(UserAdd teacher);
}

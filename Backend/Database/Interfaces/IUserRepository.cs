using Database.Models;

namespace Database.Interfaces;

public interface IUserRepository : IGenericRepository<UserBaseModel>
{
    Task<StudentModel?> GetStudentAsync(Guid id);
    Task<TeacherModel?> GetTeacherAsync(Guid id);
    Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id);
}

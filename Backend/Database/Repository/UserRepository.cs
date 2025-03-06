using Database.DTO.User;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class UserRepository(Context context, ILoggerFactory loggerFactory) : IUserRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<UserBaseModel> _genericRepository = new(context, loggerFactory);

    public async Task<User> GetStudentAsync(Guid id)
    {
        StudentModel student = await _context.Users.OfType<StudentModel>().FirstOrDefaultAsync(t => t.Guid == id) ?? throw new Exception("Student not found.");
        return student.ToDto();
    }

    public async Task<User> GetTeacherAsync(Guid id)
    {
        TeacherModel teacher = await _context.Users.OfType<TeacherModel>().FirstOrDefaultAsync(u => u.Guid == id) ?? throw new Exception("Teacher not found.");
        return teacher.ToDto();
    }

    public async Task<User> GetUserAsync(Guid id)
    {
        UserBaseModel user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == id) ?? throw new Exception("User not found.");
        return user.ToDto();
    }

    public async Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id)
    {
        UserBaseModel user = await _genericRepository.GetSingleAsync(u => u.Guid == id, query => query.Include(u => (u as StudentModel).ActiveQuestionnaires))
            ?? throw new Exception("User not found");

        return user.ActiveQuestionnaires.OrderBy(a => a.ActivatedAt).Select(a => a.Id).FirstOrDefault();
    }

    public bool UserExists(Guid id)
    {
        return _genericRepository.Count(u => u.Guid == id) != 0;
    }

    public bool UserExists(int primaryKey)
    {
        return _genericRepository.Count(u => u.Id == primaryKey) != 0;
    }

    public async Task AddStudentAsync(UserAdd student)
    {
        await _genericRepository.AddAsync(student.ToStudentModel());
    }

    public async Task AddTeacherAsync(UserAdd teacher)
    {
        await _genericRepository.AddAsync(teacher.ToTeacherModel());
    }
}

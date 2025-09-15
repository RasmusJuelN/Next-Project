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

    public async Task<FullUser?> GetStudentAsync(Guid id)
    {
        StudentModel? student = await _context.Users.OfType<StudentModel>().FirstOrDefaultAsync(t => t.Guid == id);
        return student?.ToDto();
    }

    public async Task<FullUser?> GetTeacherAsync(Guid id)
    {
        TeacherModel? teacher = await _context.Users.OfType<TeacherModel>().FirstOrDefaultAsync(u => u.Guid == id);
        return teacher?.ToDto();
    }

    public async Task<FullUser?> GetUserAsync(Guid id)
    {
        UserBaseModel? user = await _context.Users.FirstOrDefaultAsync(u => u.Guid == id);
        return user?.ToDto();
    }

    // for get all student and teacher 
    //public async Task<List<FullUser>> GetAllTeachersAsync()
    //{
    //    return await _context.Users
    //        .OfType<TeacherModel>()
    //        .Select(t => t.ToDto())
    //        .ToListAsync();
    //}

    //public async Task<List<FullUser>> GetAllStudentsAsync()
    //{
    //    return await _context.Users
    //        .OfType<StudentModel>()
    //        .Select(s => s.ToDto())
    //        .ToListAsync();
    //}




    public async Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id)
    {
        return await _context.ActiveQuestionnaires
            .Where(a => a.Student.Guid == id && !a.StudentCompletedAt.HasValue || a.Teacher.Guid == id && !a.TeacherCompletedAt.HasValue)
            .OrderBy(a => a.ActivatedAt)
            .Select(a => (Guid?)a.Id)
            .FirstOrDefaultAsync();
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

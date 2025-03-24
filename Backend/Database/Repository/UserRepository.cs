using Database.DTO.ActiveQuestionnaire;
using Database.DTO.User;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

    public async Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id)
    {
        return await _context.ActiveQuestionnaires
            .Where(a => a.Student.Guid == id && !a.StudentCompletedAt.HasValue || a.Teacher.Guid == id && !a.TeacherCompletedAt.HasValue)
            .OrderBy(a => a.ActivatedAt)
            .Select(a => (Guid?)a.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserSpecificActiveQuestionnaireBase>> GetAllAssociatedActiveQuestionnaires(Guid userId)
    {
        UserBaseModel user = await _context.Users.SingleOrDefaultAsync(u => u.Guid == userId) ?? throw new ArgumentException("User not found");

        IIncludableQueryable<UserBaseModel, ICollection<ActiveQuestionnaireModel>> include;
        bool IsStudent;
        if (user.GetType().Equals(typeof(StudentModel)))
        {
            include = _context.Users.Include(u => ((StudentModel)u).ActiveQuestionnaires);
            IsStudent = true;
        }
        else if (user.GetType().Equals(typeof(TeacherModel)))
        {
            include = _context.Users.Include(u => ((TeacherModel)u).ActiveQuestionnaires);
            IsStudent = false;
        }
        else
        {
            throw new ArgumentException("User is neither student nor teacher");
        }

        List<ActiveQuestionnaireModel> activeQuestionnaires = [.. (await include
            .Where(u => u.Guid == userId)
            .Select(u => u.ActiveQuestionnaires)
            .ToListAsync())
            .SelectMany(a => a)];
        
        if (IsStudent)
        {
            return [.. activeQuestionnaires.Select(a => a.ToBaseDTOAsStudent())];
        }
        else
        {
            return [.. activeQuestionnaires.Select(a => a.ToBaseDTOAsTeacher())];
        }
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

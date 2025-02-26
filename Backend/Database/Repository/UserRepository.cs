using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class UserRepository(Context context, ILoggerFactory loggerFactory) : SQLGenericRepository<UserBaseModel>(context, loggerFactory), IUserRepository
{
    private readonly Context _context = context;

    public async Task<StudentModel?> GetStudentAsync(Guid id)
    {
        return await _context.Users.OfType<StudentModel>().FirstOrDefaultAsync(t => t.Guid == id);
    }

    public async Task<TeacherModel?> GetTeacherAsync(Guid id)
    {
        return await _context.Users.OfType<TeacherModel>().FirstOrDefaultAsync(u => u.Guid == id);
    }

    public async Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id)
    {
        UserBaseModel user = await GetSingleAsync(u => u.Guid == id, query => query.Include(u => u.ActiveQuestionnaires))
            ?? throw new Exception("User not found");

        return user.ActiveQuestionnaires.OrderBy(a => a.ActivatedAt).Select(a => a.Id).FirstOrDefault();
    }
}

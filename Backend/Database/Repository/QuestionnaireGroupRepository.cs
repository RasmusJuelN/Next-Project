using System;
using System.Threading.Tasks;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repository
{
    public class QuestionnaireGroupRepository : IQuestionnaireGroupRepository
    {
        private readonly Context _context;
        public QuestionnaireGroupRepository(Context context)
        {
            _context = context;
        }

        public async Task AddAsync(QuestionnaireGroupModel group)
        {
            _context.Set<QuestionnaireGroupModel>().Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task<QuestionnaireGroupModel> GetByIdAsync(Guid groupId)
        {
            return await _context.Set<QuestionnaireGroupModel>().FindAsync(groupId);
        }
        public async Task<IEnumerable<QuestionnaireGroupModel>> GetAllAsync()
        {
            return await _context.QuestionnaireGroups
                .Include(g => g.Questionnaires)
                    .ThenInclude(q => q.Student)
                .Include(g => g.Questionnaires)
                    .ThenInclude(q => q.Teacher)
                .Include(g => g.Questionnaires)
                    .ThenInclude(q => q.QuestionnaireTemplate)
                .ToListAsync();
        }
    }
}
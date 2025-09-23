using System;
using System.Threading.Tasks;
using Database.DTO.ActiveQuestionnaire;
using Database.Enums;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Database.Enums;
using Microsoft.Extensions.Logging;

namespace Database.Repository
{
    public class QuestionnaireGroupRepository : IQuestionnaireGroupRepository
    {
        private readonly Context _context;
        private readonly GenericRepository<QuestionnaireGroupModel> _genericRepository;

        public QuestionnaireGroupRepository(Context context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _genericRepository = new GenericRepository<QuestionnaireGroupModel>(context, loggerFactory);
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
        public async Task<(List<QuestionnaireGroupModel>, int)> PaginationQueryWithKeyset(
    int amount,
    QuestionnaireGroupOrderingOptions sortOrder,
    Guid? cursorIdPosition = null,
    DateTime? cursorCreatedAtPosition = null,
    string? titleQuery = null,
    Guid? groupId = null,
    bool? pendingStudent = false,
        bool? pendingTeacher = false)
        {
            IQueryable<QuestionnaireGroupModel> query = _genericRepository.GetAsQueryable();

            query = sortOrder.ApplyQueryOrdering(query);

            if (!string.IsNullOrEmpty(titleQuery))
            {
                query = query.Where(g => g.Name.Contains(titleQuery));
            }

            if (groupId is not null)
            {
                query = query.Where(g => g.GroupId == groupId);
            }
            if (pendingStudent == true)
        query = query.Where(g => g.Questionnaires.Any(q => !q.StudentCompletedAt.HasValue));
    if (pendingTeacher == true)
        query = query.Where(g => g.Questionnaires.Any(q => !q.TeacherCompletedAt.HasValue));

            int totalCount = await query.CountAsync();

            if (cursorIdPosition is not null && cursorCreatedAtPosition is not null)
            {
                if (sortOrder == QuestionnaireGroupOrderingOptions.CreatedAtAsc)
                {
                    query = query.Where(g =>
                        g.CreatedAt > cursorCreatedAtPosition
                        || (g.CreatedAt == cursorCreatedAtPosition && g.GroupId > cursorIdPosition));
                }
                else
                {
                    query = query.Where(g =>
                        g.CreatedAt < cursorCreatedAtPosition
                        || (g.CreatedAt == cursorCreatedAtPosition && g.GroupId < cursorIdPosition));
                }
            }

            List<QuestionnaireGroupModel> groupEntities = await query
                .Include(g => g.Template)
                .Include(g => g.Questionnaires)
                    .ThenInclude(q => q.Student)
                .Include(g => g.Questionnaires)
                    .ThenInclude(q => q.Teacher)
                .Take(amount)
                .ToListAsync();

            return (groupEntities, totalCount);
        }



    }
}
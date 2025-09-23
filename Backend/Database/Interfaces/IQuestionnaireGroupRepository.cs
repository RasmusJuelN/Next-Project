using Database.DTO.ActiveQuestionnaire;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Enums;

namespace Database.Interfaces
{
    public interface IQuestionnaireGroupRepository
    {
        Task AddAsync(QuestionnaireGroupModel group);
        Task<IEnumerable<QuestionnaireGroupModel>> GetAllAsync();
        Task<QuestionnaireGroupModel> GetByIdAsync(Guid groupId);
        Task<(List<QuestionnaireGroupModel>, int)> PaginationQueryWithKeyset(
        int amount,
        QuestionnaireGroupOrderingOptions sortOrder,
        Guid? cursorIdPosition = null,
        DateTime? cursorCreatedAtPosition = null,
        string? titleQuery = null,
        Guid? groupId = null,
        bool? pendingStudent = false,
        bool? pendingTeacher = false);
        
    }
}

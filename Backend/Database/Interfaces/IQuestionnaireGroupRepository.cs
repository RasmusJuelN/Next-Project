using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Interfaces
{
    public interface IQuestionnaireGroupRepository
    {
        Task AddAsync(QuestionnaireGroupModel group);
        Task<QuestionnaireGroupModel> GetByIdAsync(Guid groupId);
        // Add more methods as needed (e.g., List, Update, Delete)
    }
}

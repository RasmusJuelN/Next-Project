using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.DTO.ActiveQuestionnaire
{
    public record class QuestionnaireGroup : QuestionnaireGroupBase
    {
        public List<ActiveQuestionnaireModel> Questionnaires { get; set; } = new();
    }
}

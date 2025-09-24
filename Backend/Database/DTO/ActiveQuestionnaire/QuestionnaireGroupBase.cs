using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.DTO.ActiveQuestionnaire
{
    public record class QuestionnaireGroupBase
    {
        public Guid GroupId { get; set; }
        public Guid TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Optional: include template title or questionnaire count if frontend needs it
        public string? TemplateTitle { get; set; }
        public int QuestionnaireCount { get; set; }
    }
}

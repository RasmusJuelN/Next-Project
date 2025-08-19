using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public class QuestionnaireGroupModel
    {
        [Key]
        public Guid GroupId { get; set; } // Unique identifier for the group
        public Guid TemplateId { get; set; } // The template assigned to this group
        public string Name { get; set; } // User-defined name for the group
        public DateTime CreatedAt { get; set; } // Timestamp for creation
        // Optionally, add navigation properties if using EF Core
        public QuestionnaireTemplateModel Template { get; set; }
        public ICollection<ActiveQuestionnaireModel> Questionnaires { get; set; }
    }
}

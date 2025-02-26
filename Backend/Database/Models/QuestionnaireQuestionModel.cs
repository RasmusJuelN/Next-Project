using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

[Table("QuestionnaireTemplateQuestion")]
public class QuestionnaireQuestionModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public required string Prompt { get; set; }
    
    [Required]
    public required bool AllowCustom { get; set; }
    
    [Required]
    public Guid QuestionnaireTemplateFK { get; set; }

    // External navigational properties and references
    [Required]
    [ForeignKey(nameof(QuestionnaireTemplateFK))]
    public virtual QuestionnaireTemplateModel? QuestionnaireTemplate { get; set; }
    
    public virtual ICollection<QuestionnaireOptionModel> Options { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

[Table("QuestionnaireTemplateOption")]
public class QuestionnaireOptionModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required int OptionValue { get; set; }
    
    [Required]
    [MaxLength(250)]
    public required string DisplayText { get; set; }
    
    [Required]
    public int QuestionFK { get; set; }

    // External navigational properties and references
    
    [Required]
    [ForeignKey(nameof(QuestionFK))]
    public virtual QuestionnaireQuestionModel? Question { get; set; }
}

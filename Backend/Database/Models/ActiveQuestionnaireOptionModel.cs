using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

[Table("ActiveQuestionnaireOption")]
public class ActiveQuestionnaireOptionModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required int OptionValue { get; set; }
    
    [Required]
    public required string DisplayText { get; set; }
    
    [Required]
    public int ActiveQuestionnaireQuestionFK { get; set; }

    // External navigational properties and references
    [Required]
    [ForeignKey(nameof(ActiveQuestionnaireQuestionFK))]
    public virtual ActiveQuestionnaireQuestionModel? ActiveQuestionnaireQuestion { get; set; }
}

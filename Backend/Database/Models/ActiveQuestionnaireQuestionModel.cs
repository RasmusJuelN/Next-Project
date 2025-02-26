using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

[Table("ActiveQuestionnaireQuestion")]
public class ActiveQuestionnaireQuestionModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required string Prompt { get; set; }
    
    [Required]
    public Guid ActiveQuestionnaireFK { get; set; }

    // External navigational properties and references
    [Required]
    [ForeignKey(nameof(ActiveQuestionnaireFK))]
    public virtual ActiveQuestionnaireModel? ActiveQuestionnaire { get; set; }
    
    public virtual ICollection<ActiveQuestionnaireOptionModel> ActiveQuestionnaireOptions { get; set; } = [];
}

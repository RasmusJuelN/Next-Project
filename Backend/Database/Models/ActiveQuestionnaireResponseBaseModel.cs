using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

[Table("ActiveQuestionnaireResponse")]
public class ActiveQuestionnaireResponseBaseModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int QuestionFK { get; set; }
    
    public int? OptionFK { get; set; }
    
    public string? CustomResponse { get; set; }
    
    [Required]
    public Guid ActiveQuestionnaireFK { get; set; }

    // Navigational properties and references
    [Required]
    [ForeignKey(nameof(QuestionFK))]
    public virtual QuestionnaireQuestionModel? Question { get; set; }

    [ForeignKey(nameof(OptionFK))]
    public virtual QuestionnaireOptionModel? Option { get; set; }

    [Required]
    [ForeignKey(nameof(ActiveQuestionnaireFK))]
    public virtual ActiveQuestionnaireModel? ActiveQuestionnaire { get; set; }
}

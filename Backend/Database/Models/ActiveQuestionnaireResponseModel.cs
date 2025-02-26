using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

[Table("ActiveQuestionnaireResponse")]
public class ActiveQuestionnaireResponseModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ActiveQuestionnaireQuestionFK { get; set; }
    
    [Required]
    public Guid ActiveQuestionnaireFK { get; set; }
    
    public string? StudentResponse { get; set; }
    public string? TeacherResponse { get; set; }
    
    public int? CustomStudentResponseFK { get; set; }
    public int? CustomTeacherResponseFK { get; set; }

    // Navigational properties and references
    [Required]
    [ForeignKey(nameof(ActiveQuestionnaireQuestionFK))]
    public virtual ActiveQuestionnaireQuestionModel? ActiveQuestionnaireQuestion { get; set; }
    
    [Required]
    [ForeignKey(nameof(ActiveQuestionnaireFK))]
    public virtual ActiveQuestionnaireModel? ActiveQuestionnaire { get; set; }
    
    [ForeignKey(nameof(CustomStudentResponseFK))]
    public virtual StudentCustomAnswerModel? CustomStudentResponse { get; set; }
    
    [ForeignKey(nameof(CustomTeacherResponseFK))]
    public virtual TeacherCustomAnswerModel? CustomTeacherResponse { get; set; }
}

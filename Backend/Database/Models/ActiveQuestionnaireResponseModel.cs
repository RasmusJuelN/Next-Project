using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

[Table("ActiveQuestionnaireResponse")]
public class ActiveQuestionnaireResponseModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Question { get; set; }
    
    public string? StudentResponse { get; set; }
    public string? TeacherResponse { get; set; }
    
    public string? CustomStudentResponse { get; set; }
    public string? CustomTeacherResponse { get; set; }
    
    [Required]
    public Guid ActiveQuestionnaireFK { get; set; }

    // Navigational properties and references
    [Required]
    [ForeignKey(nameof(ActiveQuestionnaireFK))]
    public virtual ActiveQuestionnaireModel? ActiveQuestionnaire { get; set; }
}

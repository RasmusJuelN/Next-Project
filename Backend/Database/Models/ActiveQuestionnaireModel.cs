using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[Table("ActiveQuestionnaire")]
[Index(nameof(Title))]
public class ActiveQuestionnaireModel
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public required string Title { get; set; }

    public string? Description { get; set; }
    
    [Required]
    public int StudentFK { get; set; }
    
    [Required]
    public int TeacherFK { get; set; }
    
    [Required]
    public Guid QuestionnaireTemplateFK { get; set; }

    [Required]
    public Guid GroupId { get; set; }

    // Default value configured in Fluent API
    [Required]
    public DateTime ActivatedAt { get; set; }
    
    public DateTime? StudentCompletedAt { get; set; }
    public DateTime? TeacherCompletedAt { get; set; }
    
    // Navigational properties and references
    [Required]
    [ForeignKey(nameof(StudentFK))]
    public required StudentModel Student { get; set; }
    
    [Required]
    [ForeignKey(nameof(TeacherFK))]
    public required TeacherModel Teacher { get; set; }
    
    [Required]
    [ForeignKey(nameof(QuestionnaireTemplateFK))]
    public required QuestionnaireTemplateModel QuestionnaireTemplate { get; set; }

    [ForeignKey(nameof(GroupId))]
    public QuestionnaireGroupModel Group { get; set; }

    public virtual ICollection<ActiveQuestionnaireStudentResponseModel> StudentAnswers { get; set; } = [];
    public virtual ICollection<ActiveQuestionnaireTeacherResponseModel> TeacherAnswers { get; set; } = [];
}

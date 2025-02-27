using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[Table("QuestionnaireTemplate")]
[Index(nameof(Title), IsUnique = true)]
 // Indexes for keyset pagination
[Index(nameof(CreatedAt), nameof(Id), IsDescending = [false, false])]
[Index(nameof(CreatedAt), nameof(Id), IsDescending = [true, false])]
[Index(nameof(Title), nameof(Id), IsDescending = [false, false])]
[Index(nameof(Title), nameof(Id), IsDescending = [true, false])]
public class QuestionnaireTemplateModel
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(150)]
    public required string Title { get; set; }

    public string? Description { get; set; }
    
    // Default value configured in Fluent API
    [Required]
    public DateTime CreatedAt { get; set; }
    
    // Default value configured in Fluent API
    [Required]
    public DateTime LastUpated { get; set; }
    
    // Figure out if and what we're gonna use this for
    [Required]
    public bool IsLocked { get; set; }

    // Navigational properties and references
    public virtual ICollection<QuestionnaireQuestionModel> Questions { get; set; } = [];

    public virtual ICollection<ActiveQuestionnaireModel> ActiveQuestionnaires { get; set; } = [];
}
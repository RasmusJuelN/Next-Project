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
    private readonly DbContext? _context;
    
    public QuestionnaireTemplateModel() {}
    public QuestionnaireTemplateModel(DbContext context)
    {
        _context = context;
    }
    
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(150)]
    public required string Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
    
    // Default value configured in Fluent API
    [Required]
    public DateTime CreatedAt { get; set; }
    
    // Default value configured in Fluent API
    [Required]
    public DateTime LastUpated { get; set; }
    
    public bool IsLocked => _context?.Set<ActiveQuestionnaireModel>().Where(q => Id == q.QuestionnaireTemplateFK).Any() ?? false;
    public TemplateStatus templateStatus { get; set; } = TemplateStatus.Draft;

    // Navigational properties and references
    public virtual ICollection<QuestionnaireQuestionModel> Questions { get; set; } = [];

    public virtual ICollection<ActiveQuestionnaireModel> ActiveQuestionnaires { get; set; } = [];
}
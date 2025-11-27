
namespace Database.Models;

/// <summary>
/// Represents a questionnaire template that serves as a blueprint for creating active questionnaires.
/// Templates define the structure, questions, and options that will be used when questionnaires are activated.
/// </summary>
/// <remarks>
/// Templates are reusable structures that can be activated multiple times for different student-teacher pairs.
/// Once a template has active questionnaires, it becomes locked to prevent modifications that could
/// affect ongoing questionnaire instances. Multiple indexes support efficient querying and keyset pagination.
/// </remarks>
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
    
    /// <summary>
    /// Initializes a new instance of the QuestionnaireTemplateModel class.
    /// </summary>
    public QuestionnaireTemplateModel() {}
    
    /// <summary>
    /// Initializes a new instance of the QuestionnaireTemplateModel class with database context.
    /// </summary>
    /// <param name="context">The database context used for calculating computed properties like IsLocked.</param>
    /// <remarks>
    /// The context parameter enables the calculation of the IsLocked property by querying related active questionnaires.
    /// </remarks>
    public QuestionnaireTemplateModel(DbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Gets or sets the unique identifier for this questionnaire template.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the title of the questionnaire template.
    /// </summary>
    /// <remarks>
    /// Maximum length of 150 characters. Must be unique across all templates.
    /// Used for identification and display in template lists and active questionnaires.
    /// </remarks>
    [Required]
    [MaxLength(150)]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the optional description providing additional context about the questionnaire template.
    /// </summary>
    /// <remarks>
    /// Maximum length of 500 characters. Provides detailed information about the template's
    /// purpose, intended use, or special instructions for completion.
    /// </remarks>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when this template was created.
    /// </summary>
    /// <remarks>
    /// Default value is configured in Fluent API to use the current timestamp at creation.
    /// Used for chronological ordering and keyset pagination operations.
    /// </remarks>
    [Required]
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when this template was last updated.
    /// </summary>
    /// <remarks>
    /// Default value is configured in Fluent API to use the current timestamp at creation and updates.
    /// Tracks modification history for audit and version control purposes.
    /// </remarks>
    [Required]
    public DateTime LastUpated { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether this template is locked and cannot be modified.
    /// </summary>
    /// <remarks>
    /// A template becomes locked when it has associated active questionnaires to prevent
    /// modifications that could affect ongoing questionnaire instances. This computed property
    /// queries the database to check for active questionnaire relationships.
    /// Requires a database context to be provided for accurate calculation.
    /// </remarks>
    public bool IsLocked => _context?.Set<StandardActiveQuestionnaireModel>().Where(q => Id == q.QuestionnaireTemplateFK).Any() ?? false;
    public TemplateStatus TemplateStatus { get; set; } = TemplateStatus.Draft;

    /// <summary>
    /// Gets or sets the collection of questions that belong to this template.
    /// </summary>
    /// <remarks>
    /// Contains all questions and their associated options that define the template structure.
    /// Virtual property enables lazy loading of questions when needed.
    /// </remarks>
    public virtual ICollection<QuestionnaireQuestionModel> Questions { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of active questionnaires created from this template.
    /// </summary>
    /// <remarks>
    /// Contains all active questionnaire instances that were created using this template.
    /// Used track template usage. Virtual property enables lazy loading of active questionnaires when needed.
    /// </remarks>
    public virtual ICollection<StandardActiveQuestionnaireModel> ActiveQuestionnaires { get; set; } = [];
}
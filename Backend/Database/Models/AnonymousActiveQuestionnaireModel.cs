namespace Database.Models;

[Index(nameof(Title))]
[Table("ActiveQuestionnaire")]
public class AnonymousActiveQuestionnaireModel
{
    private readonly Context? _context;

    public AnonymousActiveQuestionnaireModel() {}

    public AnonymousActiveQuestionnaireModel(Context context)
    {
        _context = context;
    }
    /// <summary>
    /// Gets or sets the unique identifier for this active questionnaire instance.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the title of the questionnaire, copied from the template at activation time.
    /// </summary>
    /// <remarks>
    /// This field is indexed for efficient searching.
    /// </remarks>
    [Required]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the optional description of the questionnaire, copied from the template at activation time.
    /// </summary>
    /// <remarks>
    /// Provides additional context about the questionnaire purpose and content.
    /// </remarks>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key reference to the questionnaire template this active instance was created from.
    /// </summary>
    /// <remarks>
    /// Links to the QuestionnaireTemplateModel to maintain the relationship with the original template.
    /// This relationship is preserved even if the template is later modified.
    /// </remarks>
    [Required]
    public Guid QuestionnaireTemplateFK { get; set; }

    [Required]
    public Guid GroupId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this questionnaire was activated and made available for completion.
    /// </summary>
    /// <remarks>
    /// Default value is configured in Fluent API to use the current timestamp at creation.
    /// Used for tracking questionnaire age and ordering by activation time.
    /// </remarks>
    [Required]
    public DateTime ActivatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the questionnaire template this instance was created from.
    /// </summary>
    /// <remarks>
    /// Provides access to the original template structure including questions and options.
    /// Useful for referencing the template structure during questionnaire processing.
    /// </remarks>
    [Required]
    [ForeignKey(nameof(QuestionnaireTemplateFK))]
    public QuestionnaireTemplateModel? QuestionnaireTemplate { get; set; }

    [ForeignKey(nameof(GroupId))]
    public QuestionnaireGroupModel? Group { get; set; }

    /// <summary>
    /// Gets or sets the type of the active questionnaire.
    /// This property defines the category or classification of the questionnaire being used.
    /// </summary>
    /// <value>
    /// An <see cref="ActiveQuestionnaireType"/> enumeration value that specifies the questionnaire type.
    /// This property is required and must be set when creating an instance of the model.
    /// </value>
    public ActiveQuestionnaireType QuestionnaireType { get; set; }

    [Required]
    public required List<Guid> ParticipantIds { get; set; }

    [Required]
    [ForeignKey(nameof(ParticipantIds))]
    public List<UserBaseModel> Users { get; set; } = [];

    public List<Guid> CompletedParticipantIds { get; set; } = [];

    public async Task MarkParticipantCompletedAsync(Guid participantId)
    {
        if (!CompletedParticipantIds.Contains(participantId))
        {
            CompletedParticipantIds.Add(participantId);
            if (_context != null)
            {
                _context.AnonymousActiveQuestionnaires.Update(this);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task MarkParticipantIncompleteAsync(Guid participantId)
    {
        if (CompletedParticipantIds.Contains(participantId))
        {
            CompletedParticipantIds.Remove(participantId);
            if (_context != null)
            {
                _context.AnonymousActiveQuestionnaires.Update(this);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<bool> HasParticipantCompletedAsync(Guid participantId)
    {
        return CompletedParticipantIds.Contains(participantId);
    }
}

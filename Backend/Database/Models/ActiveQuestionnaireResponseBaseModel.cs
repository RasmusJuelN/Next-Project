
namespace Database.Models;

/// <summary>
/// Represents the base model for responses submitted to active questionnaires.
/// This abstract base class contains common properties shared by both student and teacher response models.
/// </summary>
/// <remarks>
/// This model uses Table Per Hierarchy (TPH) inheritance pattern where student and teacher responses
/// are stored in the same table with a discriminator column. Contains the core response data including
/// selected options and custom text responses.
/// </remarks>
[Table("ActiveQuestionnaireResponse")]
public class ActiveQuestionnaireResponseBaseModel
{
    /// <summary>
    /// Gets or sets the unique identifier for this response record.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key reference to the question this response answers.
    /// </summary>
    /// <remarks>
    /// Links to the QuestionnaireQuestionModel to identify which question this response addresses.
    /// </remarks>
    [Required]
    public int QuestionFK { get; set; }
    
    /// <summary>
    /// Gets or sets the optional foreign key reference to the selected predefined option.
    /// </summary>
    /// <remarks>
    /// When the user selects a predefined answer option, this field references that option.
    /// Null when the response is purely custom text or when no option was selected.
    /// </remarks>
    public int? OptionFK { get; set; }
    
    /// <summary>
    /// Gets or sets the optional custom text response provided by the user.
    /// </summary>
    /// <remarks>
    /// Contains free-text responses when the question allows custom answers or when
    /// the user provides additional context beyond the selected option. Can be used
    /// alongside or instead of predefined options depending on question configuration.
    /// </remarks>
    public string? CustomResponse { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key reference to the active questionnaire this response belongs to.
    /// </summary>
    /// <remarks>
    /// Links this response to the specific active questionnaire instance being completed.
    /// </remarks>
    [Required]
    public Guid ActiveQuestionnaireFK { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the question this response answers.
    /// </summary>
    /// <remarks>
    /// Provides access to the complete question information including prompt text and configuration.
    /// Virtual property enables lazy loading of question details when needed.
    /// </remarks>
    [Required]
    [ForeignKey(nameof(QuestionFK))]
    public virtual QuestionnaireQuestionModel? Question { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the selected predefined option, if any.
    /// </summary>
    /// <remarks>
    /// When a predefined option was selected, this provides access to the option details
    /// including display text and internal values. Null for custom-only responses.
    /// Virtual property enables lazy loading of option details when needed.
    /// </remarks>
    [ForeignKey(nameof(OptionFK))]
    public virtual QuestionnaireOptionModel? Option { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the active questionnaire this response belongs to.
    /// </summary>
    /// <remarks>
    /// Provides access to the complete active questionnaire context including participants and timeline.
    /// Virtual property enables lazy loading of questionnaire details when needed.
    /// </remarks>
    [Required]
    public virtual StandardActiveQuestionnaireModel? ActiveQuestionnaire { get; set; }
}

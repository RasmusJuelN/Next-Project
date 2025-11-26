using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

/// <summary>
/// Represents an anonymous response to a questionnaire question in the database.
/// </summary>
/// <remarks>
/// This model stores individual responses to questionnaire questions without identifying the respondent,
/// maintaining anonymity while preserving response data. Each record links to a specific question
/// and may reference a predefined option or contain custom response data. The model supports
/// tracking response counts for analytics while maintaining user privacy.
/// 
/// The entity is mapped to the "AnonymousQuesionnaireResponse" table and includes navigation
/// properties for related entities such as questions, options, and active questionnaires.
/// </remarks>
[Table("AnonymousQuesionnaireResponse")]
public class AnonymousQuestionnaireResponseModel
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
    /// Gets or sets the total number of responses received for the anonymous questionnaire.
    /// </summary>
    /// <value>
    /// An integer representing the count of responses submitted for this questionnaire.
    /// </value>
    public int ResponseCount { get; set; }

    // <summary>
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

/// <summary>
/// Represents a predefined answer option for a questionnaire question.
/// Provides structured response choices with both internal values and user-facing display text.
/// </summary>
/// <remarks>
/// Each option belongs to a specific question and provides a standardized response choice.
/// The separation of OptionValue and DisplayText allows for stable internal processing
/// while maintaining flexible user interface presentation.
/// </remarks>
[Table("QuestionnaireTemplateOption")]
public class QuestionnaireOptionModel
{
    /// <summary>
    /// Gets or sets the unique identifier for this option.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the internal numeric value associated with this option.
    /// </summary>
    /// <remarks>
    /// Used for data processing, scoring, and analysis. Provides a stable identifier
    /// that remains consistent even if display text is modified for presentation purposes.
    /// </remarks>
    [Required]
    public required int OptionValue { get; set; }
    
    /// <summary>
    /// Gets or sets the user-facing text displayed for this option.
    /// </summary>
    /// <remarks>
    /// Maximum length of 250 characters. This text is presented to users when selecting
    /// responses and can be modified for clarity or localization without affecting
    /// the underlying data processing logic.
    /// </remarks>
    [Required]
    [MaxLength(250)]
    public required string DisplayText { get; set; }
    
    /// <summary>
    /// Gets or sets the sort order for this option within the question.
    /// </summary>
    /// <remarks>
    /// Used to control the display order of options in the frontend. Lower values appear first.
    /// Enables drag and drop reordering functionality in the user interface.
    /// </remarks>
    [Required]
    public required int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key reference to the question this option belongs to.
    /// </summary>
    /// <remarks>
    /// Links this option to its parent question in the questionnaire structure.
    /// </remarks>
    [Required]
    public int QuestionFK { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the question this option belongs to.
    /// </summary>
    /// <remarks>
    /// Provides access to the parent question context including prompt text and configuration.
    /// Virtual property enables lazy loading of question details when needed.
    /// </remarks>
    [Required]
    [ForeignKey(nameof(QuestionFK))]
    public virtual QuestionnaireQuestionModel? Question { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

/// <summary>
/// Represents a question within a questionnaire template, including its prompt text and configuration options.
/// Questions can have predefined answer options and may optionally allow custom text responses.
/// </summary>
/// <remarks>
/// Each question belongs to a questionnaire template and defines the structure for collecting responses.
/// The AllowCustom flag determines whether users can provide free-text responses in addition to
/// or instead of selecting from predefined options.
/// </remarks>
[Table("QuestionnaireTemplateQuestion")]
public class QuestionnaireQuestionModel
{
    /// <summary>
    /// Gets or sets the unique identifier for this question.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the question text presented to users.
    /// </summary>
    /// <remarks>
    /// Maximum length of 500 characters. Should be clear and specific to elicit
    /// the desired type of response from users. Forms the primary content that
    /// users see when completing the questionnaire.
    /// </remarks>
    [Required]
    [MaxLength(500)]
    public required string Prompt { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether users can provide custom text responses to this question.
    /// </summary>
    /// <remarks>
    /// When true, users can enter free-text responses in addition to or instead of
    /// selecting from predefined options. When false, users are limited to the
    /// predefined options only. Affects the UI presentation and validation logic.
    /// </remarks>
    [Required]
    public required bool AllowCustom { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key reference to the questionnaire template this question belongs to.
    /// </summary>
    /// <remarks>
    /// Links this question to its parent questionnaire template in the hierarchical structure.
    /// </remarks>
    [Required]
    public Guid QuestionnaireTemplateFK { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the questionnaire template this question belongs to.
    /// </summary>
    /// <remarks>
    /// Provides access to the parent template context including title, description, and metadata.
    /// Virtual property enables lazy loading of template details when needed.
    /// </remarks>
    [Required]
    [ForeignKey(nameof(QuestionnaireTemplateFK))]
    public virtual QuestionnaireTemplateModel? QuestionnaireTemplate { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of predefined answer options available for this question.
    /// </summary>
    /// <remarks>
    /// Contains all structured response choices available to users for this question.
    /// Can be empty if the question only accepts custom text responses.
    /// Virtual property enables lazy loading of options when needed.
    /// </remarks>
    public virtual ICollection<QuestionnaireOptionModel> Options { get; set; } = [];
}

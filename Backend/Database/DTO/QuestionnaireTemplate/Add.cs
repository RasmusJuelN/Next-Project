namespace Database.DTO.QuestionnaireTemplate;

/// <summary>
/// Represents a data transfer object for adding a new questionnaire option.
/// </summary>
/// <remarks>
/// This record is used when creating new questionnaire options and contains
/// the essential information needed to define an option within a questionnaire.
/// </remarks>
public record class QuestionnaireOptionAdd
{
    /// <summary>
    /// Gets or sets the integer value associated with the option.
    /// </summary>
    /// <remarks>
    /// This value is typically used for scoring or identification purposes.
    /// </remarks>
    public required int OptionValue { get; set; }

    /// <summary>
    /// Gets or sets the display text for the option.
    /// </summary>
    /// <remarks>
    /// This text is what users will see when selecting an option in the questionnaire.
    /// </remarks>
    public required string DisplayText { get; set; }
}

/// <summary>
/// Represents a data transfer object for adding a new questionnaire question.
/// </summary>
/// <remarks>
/// This record is used to encapsulate the data required to create a new question
/// within a questionnaire template, including the question prompt, custom answer settings,
/// and available answer options.
/// </remarks>
public record class QuestionnaireQuestionAdd
{
    /// <summary>
    /// Gets or sets the question prompt text that will be displayed to users.
    /// </summary>
    /// <value>
    /// A string containing the question text. This property is required.
    /// </value>
    public required string Prompt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether users can provide custom answers
    /// in addition to the predefined options.
    /// </summary>
    /// <value>
    /// <c>true</c> if custom answers are allowed; otherwise, <c>false</c>.
    /// This property is required.
    /// </value>
    public required bool AllowCustom { get; set; }

    /// <summary>
    /// Gets or sets the list of predefined answer options for this question.
    /// </summary>
    /// <value>
    /// A list of <see cref="QuestionnaireOptionAdd"/> objects representing the available
    /// answer choices. Defaults to an empty list if not specified.
    /// </value>
    public List<QuestionnaireOptionAdd> Options { get; set; } = [];
}

/// <summary>
/// Represents a data transfer object for adding a new questionnaire template.
/// </summary>
/// <remarks>
/// This record is used to capture the necessary information when creating a new questionnaire template,
/// including the title, optional description, and a collection of questions to be included in the template.
/// </remarks>
public record class QuestionnaireTemplateAdd
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public List<QuestionnaireQuestionAdd> Questions { get; set; } = [];
}
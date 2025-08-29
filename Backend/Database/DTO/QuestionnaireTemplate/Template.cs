namespace Database.DTO.QuestionnaireTemplate;

/// <summary>
/// Represents a questionnaire template that contains a collection of questions.
/// Inherits from QuestionnaireTemplateBase and extends it with a list of template questions.
/// </summary>
/// <param name="Questions">The list of questions included in the questionnaire template.</param>
public record class QuestionnaireTemplate : QuestionnaireTemplateBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

/// <summary>
/// Represents a question within a questionnaire template containing the question prompt,
/// configuration options, and available answer choices.
/// </summary>
/// <param name="Id">The unique identifier for the question.</param>
/// <param name="Prompt">The text of the question to be displayed to users.</param>
/// <param name="AllowCustom">Indicates whether users can provide custom answers beyond the predefined options.</param>
/// <param name="Options">The list of predefined answer options available for this question.</param>
public record class QuestionnaireTemplateQuestion
{
    public required int Id { get; set; }
    public required string Prompt { get; set; }
    public required bool AllowCustom { get; set; }
    public required List<QuestionnaireTemplateOption> Options { get; set; }
}

/// <summary>
/// Represents an option within a questionnaire template that defines a selectable choice for users.
/// </summary>
/// <param name="Id">The unique identifier for the option.</param>
/// <param name="OptionValue">The integer value associated with the option, typically used for scoring or identification.</param>
/// <param name="DisplayText">The text that will be displayed to users for this option.</param>
public record class QuestionnaireTemplateOption
{
    public required int Id { get; set; }
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}

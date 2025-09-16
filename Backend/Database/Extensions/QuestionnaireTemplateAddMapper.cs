using Database.DTO.QuestionnaireTemplate;
using Database.Models;

namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping questionnaire template creation DTOs to database models.
/// This mapper handles the conversion from add request DTOs to Entity Framework models for database persistence.
/// </summary>
public static class QuestionnaireTemplateAddMapper
{
    /// <summary>
    /// Converts a QuestionnaireTemplateAdd DTO to a QuestionnaireTemplateModel for database persistence.
    /// </summary>
    /// <param name="questionnaire">The QuestionnaireTemplateAdd DTO containing questionnaire creation data.</param>
    /// <returns>A QuestionnaireTemplateModel ready for database insertion.</returns>
    /// <remarks>
    /// This method maps the basic questionnaire template properties and recursively converts
    /// the associated questions and options using their respective mapper methods.
    /// The created model will have system-generated values for ID, CreatedAt, and other metadata fields.
    /// </remarks>
    public static QuestionnaireTemplateModel ToModel(this QuestionnaireTemplateAdd questionnaire)
    {
        return new()
        {
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            Questions = [.. questionnaire.Questions.Select(q => q.ToModel())]
        };
    }

    /// <summary>
    /// Converts a QuestionnaireQuestionAdd DTO to a QuestionnaireQuestionModel for database persistence.
    /// </summary>
    /// <param name="question">The QuestionnaireQuestionAdd DTO containing question creation data.</param>
    /// <returns>A QuestionnaireQuestionModel ready for database insertion.</returns>
    /// <remarks>
    /// This method maps question properties including the prompt text, custom answer allowance flag,
    /// and recursively converts the associated answer options using the option mapper.
    /// The created model will have system-generated values for ID and relationship keys.
    /// </remarks>
    public static QuestionnaireQuestionModel ToModel(this QuestionnaireQuestionAdd question)
    {
        return new()
        {
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(o => o.ToModel())]
        };
    }

    /// <summary>
    /// Converts a QuestionnaireOptionAdd DTO to a QuestionnaireOptionModel for database persistence.
    /// </summary>
    /// <param name="option">The QuestionnaireOptionAdd DTO containing option creation data.</param>
    /// <returns>A QuestionnaireOptionModel ready for database insertion.</returns>
    /// <remarks>
    /// This method maps option properties including the internal option value and user-facing display text.
    /// The OptionValue is typically used for data processing while DisplayText is shown to users.
    /// The created model will have system-generated values for ID and relationship keys.
    /// </remarks>
    public static QuestionnaireOptionModel ToModel(this QuestionnaireOptionAdd option)
    {
        return new()
        {
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}

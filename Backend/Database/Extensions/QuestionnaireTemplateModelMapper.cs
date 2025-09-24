using Database.DTO.QuestionnaireTemplate;
using Database.Models;

namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping questionnaire template database models to DTOs.
/// This mapper handles conversions from Entity Framework models to response DTOs for API endpoints.
/// </summary>
public static class QuestionnaireTemplateModelMapper
{
    /// <summary>
    /// Converts a QuestionnaireTemplateModel to a QuestionnaireTemplateBase DTO containing essential information.
    /// </summary>
    /// <param name="questionnaire">The QuestionnaireTemplateModel from the database.</param>
    /// <returns>A QuestionnaireTemplateBase DTO with basic questionnaire information.</returns>
    /// <remarks>
    /// This method creates a lightweight DTO containing only the essential questionnaire template metadata
    /// without the full question structure. Useful for list views and summary displays where detailed
    /// question data is not required.
    /// </remarks>
    public static QuestionnaireTemplateBase ToBaseDto(this QuestionnaireTemplateModel questionnaire)
    {
        return new()
        {
            Id = questionnaire.Id,
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            CreatedAt = questionnaire.CreatedAt,
            LastUpdated = questionnaire.LastUpated,
            IsLocked = questionnaire.IsLocked,
            TemplateStatus = questionnaire.TemplateStatus
        };
    }

    /// <summary>
    /// Converts a QuestionnaireTemplateModel to a complete QuestionnaireTemplate DTO including all questions and options.
    /// </summary>
    /// <param name="questionnaire">The QuestionnaireTemplateModel from the database.</param>
    /// <returns>A QuestionnaireTemplate DTO with complete questionnaire structure.</returns>
    /// <remarks>
    /// This method creates a comprehensive DTO containing the full questionnaire template structure
    /// including all questions and their associated options. Used for detailed views and when the
    /// complete questionnaire structure is required for display or editing.
    /// </remarks>
    public static QuestionnaireTemplate ToDto(this QuestionnaireTemplateModel questionnaire)
    {
        return new()
        {
            Id = questionnaire.Id,
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            CreatedAt = questionnaire.CreatedAt,
            LastUpdated = questionnaire.LastUpated,
            IsLocked = questionnaire.IsLocked,
            TemplateStatus = questionnaire.TemplateStatus,
            Questions = [.. questionnaire.Questions.Select(q => q.ToDto())]
        };
    }

    /// <summary>
    /// Converts a QuestionnaireQuestionModel to a QuestionnaireTemplateQuestion DTO.
    /// </summary>
    /// <param name="question">The QuestionnaireQuestionModel from the database.</param>
    /// <returns>A QuestionnaireTemplateQuestion DTO with question details and options.</returns>
    /// <remarks>
    /// This method maps question properties and recursively converts associated answer options.
    /// Forms part of the complete questionnaire template conversion chain for detailed responses.
    /// </remarks>
    public static QuestionnaireTemplateQuestion ToDto(this QuestionnaireQuestionModel question)
    {
        return new()
        {
            Id = question.Id,
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(o => o.ToDto())]
        };
    }

    /// <summary>
    /// Converts a QuestionnaireOptionModel to a QuestionnaireTemplateOption DTO.
    /// </summary>
    /// <param name="option">The QuestionnaireOptionModel from the database.</param>
    /// <returns>A QuestionnaireTemplateOption DTO with option details.</returns>
    /// <remarks>
    /// This method maps option properties including internal values and user-facing display text.
    /// Completes the questionnaire template structure conversion by providing option-level detail.
    /// </remarks>
    public static QuestionnaireTemplateOption ToDto(this QuestionnaireOptionModel option)
    {
        return new()
        {
            Id = option.Id,
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}

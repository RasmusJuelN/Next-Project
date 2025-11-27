
namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping questionnaire template DTOs to database models.
/// This mapper handles conversions from update/read DTOs to Entity Framework models and active questionnaire creation.
/// </summary>
public static class QuestionnaireTemplateMapper
{
    /// <summary>
    /// Converts a QuestionnaireTemplate DTO to a QuestionnaireTemplateModel for database operations.
    /// </summary>
    /// <param name="questionnaire">The QuestionnaireTemplate DTO containing questionnaire data.</param>
    /// <returns>A QuestionnaireTemplateModel ready for database persistence or updates.</returns>
    /// <remarks>
    /// This method maps questionnaire template properties including timestamps and recursively converts
    /// associated questions and options. Used primarily for update operations where timestamps are preserved.
    /// </remarks>
    public static QuestionnaireTemplateModel ToModel(this QuestionnaireTemplate questionnaire)
    {
        return new()
        {
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            CreatedAt = questionnaire.CreatedAt,
            LastUpated = questionnaire.LastUpdated,
            Questions = [.. questionnaire.Questions.Select(q => q.ToModel())]
        };
    }

    /// <summary>
    /// Converts a QuestionnaireTemplateQuestion DTO to a QuestionnaireQuestionModel for database operations.
    /// </summary>
    /// <param name="question">The QuestionnaireTemplateQuestion DTO containing question data.</param>
    /// <returns>A QuestionnaireQuestionModel ready for database persistence or updates.</returns>
    /// <remarks>
    /// This method maps question properties and recursively converts associated answer options.
    /// Used in conjunction with template mapping for comprehensive questionnaire structure conversion.
    /// </remarks>
    public static QuestionnaireQuestionModel ToModel(this QuestionnaireTemplateQuestion question)
    {
        return new()
        {
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            SortOrder = question.SortOrder,
            Options = [.. question.Options.Select(o => o.ToModel())]
        };
    }

    /// <summary>
    /// Converts a QuestionnaireTemplateOption DTO to a QuestionnaireOptionModel for database operations.
    /// </summary>
    /// <param name="option">The QuestionnaireTemplateOption DTO containing option data.</param>
    /// <returns>A QuestionnaireOptionModel ready for database persistence or updates.</returns>
    /// <remarks>
    /// This method maps option properties including internal values and display text.
    /// Forms part of the complete questionnaire structure conversion chain.
    /// </remarks>
    public static QuestionnaireOptionModel ToModel(this QuestionnaireTemplateOption option)
    {
        return new()
        {
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText,
            SortOrder = option.SortOrder
        };
    }

    /// <summary>
    /// Converts a QuestionnaireTemplate DTO to an StandardActiveQuestionnaireModel.
    /// </summary>
    /// <param name="questionnaire">The QuestionnaireTemplate DTO containing the template data.</param>
    /// <param name="questionnaireModel">The persisted QuestionnaireTemplateModel from the database.</param>
    /// <param name="student">The StudentModel who will complete the questionnaire.</param>
    /// <param name="teacher">The TeacherModel who will complete the questionnaire.</param>
    /// <returns>An StandardActiveQuestionnaireModel.</returns>
    /// <remarks>
    /// This method creates a new StandardActiveQuestionnaireModel instance based on the provided template and associated student and teacher.
    /// </remarks>
    public static StandardActiveQuestionnaireModel ToActiveQuestionnaire(this QuestionnaireTemplate questionnaire, QuestionnaireTemplateModel questionnaireModel, StudentModel student, TeacherModel teacher, ActiveQuestionnaireType questionnaireType)
    {
        return new()
        {
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            Student = student,
            Teacher = teacher,
            ParticipantIds = [],
            QuestionnaireTemplate = questionnaireModel,
            QuestionnaireType = questionnaireType
        };
    }
}

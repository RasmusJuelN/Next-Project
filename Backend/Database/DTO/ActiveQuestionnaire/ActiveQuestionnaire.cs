
namespace Database.DTO.ActiveQuestionnaire;

/// <summary>
/// Represents an active questionnaire that extends the base active questionnaire functionality
/// with a collection of template questions.
/// </summary>
/// <param name="Questions">The list of questions associated with the active questionnaire.</param>
/// <remarks>
/// This record class inherits from <see cref="ActiveQuestionnaireBase"/> and adds the ability
/// to store questionnaire template questions that are associated with the active questionnaire.
/// </remarks>
public record class ActiveQuestionnaire : ActiveQuestionnaireBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

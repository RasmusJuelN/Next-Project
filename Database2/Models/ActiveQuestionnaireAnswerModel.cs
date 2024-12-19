namespace Database.Models;

internal class ActiveQuestionnaireAnswerModel
{
    internal int Id { get; set; }
    internal string? CustomAnswerText { get; set; }

    // Navigational properties and references
    internal int ActiveQuestionnaireId { get; set; }
    internal int UserId { get; set; }
    internal int QuestionId { get; set; }
    internal int OptionId { get; set; }
    internal required UserModel User { get; set; }
    internal required ActiveQuestionnaireModel ActiveQuestionnaire { get; set; }
    internal required QuestionnaireQuestionModel Question { get; set; }
    internal required QuestionnaireOptionModel Option { get; set; }
}

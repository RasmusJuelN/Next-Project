namespace Database.Models;

internal class ActiveQuestionnaireResponse
{
    internal int Id { get; set; }
    internal int QuestionId { get; set; }
    internal int ActiveQuestionnaireId { get; set; }
    internal string? StudentResponse { get; set; }
    internal string? TeacherResponse { get; set; }

    // Navigational properties and references
    internal int ActiveQuestionnaireId { get; set; }
    internal required ActiveQuestionnaire ActiveQuestionnaire { get; set; }
}

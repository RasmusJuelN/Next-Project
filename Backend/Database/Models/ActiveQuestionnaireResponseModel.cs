namespace Database.Models;

internal class ActiveQuestionnaireResponseModel
{
    internal int Id { get; set; }
    internal required int QuestionId { get; set; }
    internal required int ActiveQuestionnaireId { get; set; }
    internal string? StudentResponse { get; set; }
    internal string? TeacherResponse { get; set; }
    internal int CustomStudentResponseId { get; set; }
    internal int CustomTeacherResponseId { get; set; }

    // Navigational properties and references
    internal required ActiveQuestionnaireQuestionModel ActiveQuestionnaireQuestion { get; set; }
    internal required ActiveQuestionnaireModel ActiveQuestionnaire { get; set; }
    internal CustomAnswerModel? CustomStudentResponse { get; set; }
    internal CustomAnswerModel? CustomTeacherResponse { get; set; }
}

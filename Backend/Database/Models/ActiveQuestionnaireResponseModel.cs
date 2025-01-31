namespace Database.Models;

public class ActiveQuestionnaireResponseModel
{
    public int Id { get; set; }
    public required int QuestionId { get; set; }
    public required Guid ActiveQuestionnaireId { get; set; }
    public string? StudentResponse { get; set; }
    public string? TeacherResponse { get; set; }
    public int CustomStudentResponseId { get; set; }
    public int CustomTeacherResponseId { get; set; }

    // Navigational properties and references
    public required ActiveQuestionnaireQuestionModel ActiveQuestionnaireQuestion { get; set; }
    public required ActiveQuestionnaireModel ActiveQuestionnaire { get; set; }
    public CustomAnswerModel? CustomStudentResponse { get; set; }
    public CustomAnswerModel? CustomTeacherResponse { get; set; }
}

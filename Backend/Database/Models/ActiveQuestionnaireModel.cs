namespace Database.Models;

internal class ActiveQuestionnaireModel
{
    internal int Id { get; set; }
    internal required int StudentId { get; set; }
    internal required int TeacherId { get; set; }
    internal required Guid QuestionnaireTemplateId { get; set; }
    internal DateTime ActivatedAt { get; set; }
    internal DateTime? StudentCompletedAt { get; set; }
    internal DateTime? TeacherCompletedAt { get; set; }
    
    // Navigational properties and references
    internal required UserModel Student { get; set; }
    internal required UserModel Teacher { get; set; }
    internal required QuestionnaireTemplateModel QuestionnaireTemplate { get; set; }
    internal required ICollection<ActiveQuestionnaireQuestionModel> ActiveQuestionnaireQuestions { get; set; }
    internal required ICollection<ActiveQuestionnaireResponseModel> Answers { get; set; }
}

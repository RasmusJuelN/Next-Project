namespace Database.Models;

public class ActiveQuestionnaireModel
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required Guid StudentId { get; set; }
    public required Guid TeacherId { get; set; }
    public required Guid QuestionnaireTemplateId { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime? StudentCompletedAt { get; set; }
    public DateTime? TeacherCompletedAt { get; set; }
    
    // Navigational properties and references
    public required UserModel Student { get; set; }
    public required UserModel Teacher { get; set; }
    public required QuestionnaireTemplateModel QuestionnaireTemplate { get; set; }
    public required ICollection<ActiveQuestionnaireQuestionModel> ActiveQuestionnaireQuestions { get; set; }
    public required ICollection<ActiveQuestionnaireResponseModel> Answers { get; set; }
}

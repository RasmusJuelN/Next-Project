namespace Database.Models;

internal class ActiveQuestionnaireModel
{
    internal int Id { get; set; }
    internal DateTime CreatedAt { get; set; }
    internal DateTime? StudentFinishedAt { get; set; }
    internal DateTime? TeacherFinishedAt { get; set; }
    
    // Navigational properties and references
    internal int StudentId { get; set; }
    internal int TeacherId { get; set; }
    internal required string TemplateId { get; set; }
    internal required UserModel Student { get; set; }
    internal required UserModel Teacher { get; set; }
    internal required QuestionnaireTemplateModel QuestionnaireTemplate { get; set; }
    internal required ICollection<ActiveQuestionnaireAnswerModel> Answers { get; set; }
}

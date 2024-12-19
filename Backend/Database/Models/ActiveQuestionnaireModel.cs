namespace Database.Models;

internal class ActiveQuestionnaire
{
    internal int Id { get; set; }
    internal int StudentId { get; set; }
    internal int TeacherId { get; set; }
    internal int QuestionnaireTemplateId { get; set; }
    internal DateTime ActivatedAt { get; set; }
    internal DateTime? StudentCompletedAt { get; set; }
    internal DateTime? TeacherCompletedAt { get; set; }
    
    // TODO: Change to reflect new database model
    // Navigational properties and references
    internal required UserModel Student { get; set; }
    internal required UserModel Teacher { get; set; }
    internal required QuestionnaireTemplate QuestionnaireTemplate { get; set; }
    internal required ICollection<ActiveQuestionnaireAnswer> Answers { get; set; }
}

namespace Database.Models;

internal class QuestionnaireTemplate
{
    internal required Guid Id { get; set; }
    internal required string TemplateTitle { get; set; }
    internal DateTime CreatedAt { get; set; }
    internal DateTime LastUpated { get; set; }
    internal bool IsLocked { get; set; }

    // TODO: Change to reflect new database model
    // Navigational properties and references
    internal required ICollection<QuestionnaireQuestion> Questions { get; set; }
    internal ICollection<ActiveQuestionnaire> ?ActiveQuestionnaires { get; set; }
}
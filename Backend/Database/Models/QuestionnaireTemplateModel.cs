namespace Database.Models;

internal class QuestionnaireTemplateModel
{
    internal required Guid Id { get; set; }
    internal required string TemplateTitle { get; set; }
    internal DateTime CreatedAt { get; set; }
    internal DateTime LastUpated { get; set; }
    internal bool IsLocked { get; set; }

    // Navigational properties and references
    internal required ICollection<QuestionnaireQuestionModel> Questions { get; set; }
    internal ICollection<ActiveQuestionnaireModel>? ActiveQuestionnaires { get; set; }
}
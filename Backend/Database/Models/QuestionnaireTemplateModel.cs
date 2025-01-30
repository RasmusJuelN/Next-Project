namespace Database.Models;

public class QuestionnaireTemplateModel
{
    public required Guid Id { get; set; }
    public required string TemplateTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpated { get; set; }
    public bool IsLocked { get; set; }

    // Navigational properties and references
    public required ICollection<QuestionnaireQuestionModel> Questions { get; set; }
    public ICollection<ActiveQuestionnaireModel>? ActiveQuestionnaires { get; set; }
}
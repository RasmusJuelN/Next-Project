namespace Database.Models;

public class QuestionnaireTemplateModel
{
    public Guid Id { get; set; }
    public required string TemplateTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpated { get; set; }
    public bool IsLocked { get; set; }

    // Navigational properties and references
    public ICollection<QuestionnaireQuestionModel> Questions { get; set; } = [];
    public ICollection<ActiveQuestionnaireModel>? ActiveQuestionnaires { get; set; } = [];
}
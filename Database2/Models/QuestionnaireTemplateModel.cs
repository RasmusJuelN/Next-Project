namespace Database.Models;

internal class QuestionnaireTemplateModel
{
    internal QuestionnaireTemplateModel()
    {
        this.ActiveQuestionnaires = new HashSet<ActiveQuestionnaireModel>();
    }
    internal string? Id { get; set; }
    internal required string TemplateTitle { get; set; }
    internal DateTime CreatedAt { get; set; }
    internal DateTime LastUpated { get; set; }

    // Navigational properties and references
    internal required ICollection<QuestionnaireQuestionModel> Questions { get; set; }
    internal ICollection<ActiveQuestionnaireModel> ActiveQuestionnaires { get; set; }
}
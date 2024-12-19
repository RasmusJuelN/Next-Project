namespace Database.Models;

internal class QuestionnaireQuestionModel
{
    internal int Id { get; set; }
    internal required string QuestionTitle { get; set; }

    // Navigational properties and references
    internal required string QuestionnaireTemplateId { get; set; }
    internal required QuestionnaireTemplateModel QuestionnaireTemplate { get; set; }
    internal required ICollection<QuestionnaireOptionModel> Options { get; set; }
}

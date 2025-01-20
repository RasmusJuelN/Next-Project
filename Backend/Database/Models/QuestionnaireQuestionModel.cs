namespace Database.Models;

internal class QuestionnaireQuestionModel
{
    internal int Id { get; set; }
    internal required string Prompt { get; set; }
    internal required bool AllowCustom { get; set; }
    internal required Guid QuestionnaireTemplateId { get; set; }

    // External navigational properties and references
    internal required QuestionnaireTemplateModel QuestionnaireTemplate { get; set; }
    internal required ICollection<QuestionnaireOptionModel> Options { get; set; }
}

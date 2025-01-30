namespace Database.Models;

public class QuestionnaireQuestionModel
{
    public int Id { get; set; }
    public required string Prompt { get; set; }
    public required bool AllowCustom { get; set; }
    public required Guid QuestionnaireTemplateId { get; set; }

    // External navigational properties and references
    public required QuestionnaireTemplateModel QuestionnaireTemplate { get; set; }
    public required ICollection<QuestionnaireOptionModel> Options { get; set; }
}

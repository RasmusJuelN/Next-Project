namespace Database.Models;

public class QuestionnaireOptionModel
{
    public int Id { get; set; }
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
    public required int QuestionId { get; set; }

    // External navigational properties and references
    public required QuestionnaireQuestionModel Question { get; set; }
}

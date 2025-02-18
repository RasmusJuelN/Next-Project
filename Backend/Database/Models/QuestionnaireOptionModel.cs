namespace Database.Models;

public class QuestionnaireOptionModel
{
    public int Id { get; set; }
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
    public int QuestionId { get; set; }

    // External navigational properties and references
    public QuestionnaireQuestionModel? Question { get; set; }
}

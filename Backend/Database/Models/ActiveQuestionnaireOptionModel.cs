namespace Database.Models;

public class ActiveQuestionnaireOptionModel
{
    public int Id { get; set; }
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
    public required int ActiveQuestionnaireQuestionId { get; set; }

    // External navigational properties and references
    public required ActiveQuestionnaireQuestionModel ActiveQuestionnaireQuestion { get; set; }
}

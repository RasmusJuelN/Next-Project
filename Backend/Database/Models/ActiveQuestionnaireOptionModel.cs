namespace Database.Models;

internal class ActiveQuestionnaireOptionModel
{
    internal int Id { get; set; }
    internal required int OptionValue { get; set; }
    internal required string DisplayText { get; set; }
    internal required int ActiveQuestionnaireQuestionId { get; set; }

    // External navigational properties and references
    internal required ActiveQuestionnaireQuestionModel ActiveQuestionnaireQuestion { get; set; }
}

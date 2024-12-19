namespace Database.Models;

internal class QuestionnaireOptionModel
{
    internal int Id { get; set; }
    internal required int OptionValue { get; set; }
    internal required string OptionText { get; set; }
    internal bool IsCustom { get; set; }

    // Navigational properties and references
    internal required int QuestionId { get; set; }
    internal required QuestionnaireQuestionModel Question { get; set; }
}

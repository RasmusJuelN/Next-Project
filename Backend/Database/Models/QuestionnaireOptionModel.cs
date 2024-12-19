namespace Database.Models;

internal class QuestionnaireOption
{
    internal int Id { get; set; }
    internal required int OptionValue { get; set; }
    internal required string DisplayText { get; set; }
    internal bool IsCustom { get; set; }

    // TODO: Change to reflect new database model
    // Navigational properties and references
    internal required int QuestionId { get; set; }
    internal required QuestionnaireQuestion Question { get; set; }
}

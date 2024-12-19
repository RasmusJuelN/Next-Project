namespace Database.Models;

internal class QuestionnaireQuestion
{
    internal int Id { get; set; }
    internal required string Prompt { get; set; }

    // TODO: Change to reflect new database model
    // Navigational properties and references
    internal required string QuestionnaireTemplateId { get; set; }
    internal required QuestionnaireTemplate QuestionnaireTemplate { get; set; }
    internal required ICollection<QuestionnaireOption> Options { get; set; }
}

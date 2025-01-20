namespace Database.Models;

internal class ActiveQuestionnaireQuestionModel
{
    internal int Id { get; set; }
    internal required string Prompt { get; set; }
    internal required int ActiveQuestionnaireId { get; set; }

    // External navigational properties and references
    internal required ActiveQuestionnaireModel ActiveQuestionnaire { get; set; }
    internal required ICollection<ActiveQuestionnaireOptionModel> ActiveQuestionnaireOptions { get; set; }
}

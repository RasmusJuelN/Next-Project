namespace Database.Models;

public class ActiveQuestionnaireQuestionModel
{
    public int Id { get; set; }
    public required string Prompt { get; set; }
    public required int ActiveQuestionnaireId { get; set; }

    // External navigational properties and references
    public required ActiveQuestionnaireModel ActiveQuestionnaire { get; set; }
    public required ICollection<ActiveQuestionnaireOptionModel> ActiveQuestionnaireOptions { get; set; }
}

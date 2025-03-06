namespace Database.DTO.QuestionnaireTemplate;

public record class QuestionnaireOptionUpdate : QuestionnaireOptionAdd
{
    public int? Id { get; set; } = null;
}

public record class QuestionnaireQuestionUpdate : QuestionnaireQuestionAdd
{
    public int? Id { get; set; } = null;
    new public List<QuestionnaireOptionUpdate> Options { get; set; } = [];
}

public record class QuestionnaireTemplateUpdate : QuestionnaireTemplateAdd
{
    new public List<QuestionnaireQuestionUpdate> Questions { get; set; } = [];
}
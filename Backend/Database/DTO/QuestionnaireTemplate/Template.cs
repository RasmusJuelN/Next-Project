namespace Database.DTO.QuestionnaireTemplate;

public record class QuestionnaireTemplate : QuestionnaireTemplateBase
{
    public required List<QuestionnaireTemplateQuestion> Questions { get; set; }
}

public record class QuestionnaireTemplateQuestion
{
    public required int Id { get; set; }
    public required string Prompt { get; set; }
    public required bool AllowCustom { get; set; }
    public required List<QuestionnaireTemplateOption> Options { get; set; }
}

public record class QuestionnaireTemplateOption
{
    public required int Id { get; set; }
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}

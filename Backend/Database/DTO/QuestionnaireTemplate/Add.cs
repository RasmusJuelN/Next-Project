namespace Database.DTO.QuestionnaireTemplate;

public record class QuestionnaireOptionAdd
{
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}

public record class QuestionnaireQuestionAdd
{
    public required string Prompt { get; set; }
    public required bool AllowCustom { get; set; }
    public List<QuestionnaireOptionAdd> Options { get; set; } = [];
}

public record class QuestionnaireTemplateAdd
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<QuestionnaireQuestionAdd> Questions { get; set; } = [];
}
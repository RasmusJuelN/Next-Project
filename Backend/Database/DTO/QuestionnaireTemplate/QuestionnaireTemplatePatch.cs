namespace Database.DTO.QuestionnaireTemplate;

public record class QuestionnaireTemplatePatch
{
    public string? TemplateTitle { get; set; }
    public string? Description { get; set; }
    public List<QuestionnaireQuestionPatch>? Questions { get; set; }
}

public record class QuestionnaireQuestionPatch
{
    public int Id { get; set; }
    public string? Prompt { get; set; }
    public bool? AllowCustom { get; set; }
    public List<QuestionnaireOptionPatch>? Options { get; set; }
}

public record class QuestionnaireOptionPatch
{
    public int Id { get; set; }
    public int? OptionValue { get; set; }
    public string? DisplayText { get; set; }
}
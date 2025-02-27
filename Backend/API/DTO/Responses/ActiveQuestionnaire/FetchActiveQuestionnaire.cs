namespace API.DTO.Responses.ActiveQuestionnaire;

public record class FetchActiveQuestionnaire : FetchActiveQuestionnaireBase
{
    public required List<FetchActiveQuestionnaireQuestion> Questions { get; set; }
}

public record class FetchActiveQuestionnaireQuestion
{
    public required int Id { get; set; }
    public required string Prompt { get; set; }
    public required bool AllowCustom { get; set; }
    public required List<FetchActiveQuestionnaireOption> Options { get; set; }
}

public record class FetchActiveQuestionnaireOption
{
    public required int Id { get; set; }
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}
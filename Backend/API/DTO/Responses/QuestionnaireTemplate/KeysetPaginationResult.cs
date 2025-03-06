namespace API.DTO.Responses.QuestionnaireTemplate;

public record class KeysetPaginationResult
{
    public List<FetchTemplateBase> TemplateBases { get; set; } = [];
    public string? QueryCursor { get; set; }
    public int TotalCount { get; set; }
}

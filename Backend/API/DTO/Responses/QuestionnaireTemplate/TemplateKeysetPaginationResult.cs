using Database.DTO.QuestionnaireTemplate;

namespace API.DTO.Responses.QuestionnaireTemplate;

public record class TemplateKeysetPaginationResult
{
    public List<QuestionnaireTemplateBase> TemplateBases { get; set; } = [];
    public string? QueryCursor { get; set; }
    public int TotalCount { get; set; }
}

namespace API.Models.Responses;

public record class QuestionnaireTemplateBaseDto
{
    public record class PaginationResult
    {
        public List<TemplateBase> TemplateBases { get; set; } = [];
        public string? QueryCursor { get; set; }
        public int TotalCount { get; set; }
    }
    
    public record class TemplateBase
    {
        public required Guid Id { get; set; }
        public required string TemplateTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLocked { get; set; }
    }
}

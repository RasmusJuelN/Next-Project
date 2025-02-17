using API.Enums;

namespace API.Models.Requests;

public record class QuestionnaireTemplatePaginationRequest
{
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public string? Title { get; set; }
    public Guid? Id { get; set; }
}

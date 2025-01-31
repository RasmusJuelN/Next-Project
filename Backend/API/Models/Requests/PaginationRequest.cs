namespace API.Models.Requests;

public record class PaginationRequest
{
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string SearchType { get; set; } = string.Empty;
}

using API.Enums;

namespace API.Models.Requests;

public record class PaginationRequest
{
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public string? SearchTerm { get; set; }
    public SearchTypes? SearchType { get; set; }
}

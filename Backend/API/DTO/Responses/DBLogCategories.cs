namespace API.Models.Responses;

public record class DBLogCategories
{
    public required List<string> Categories { get; set; }
}

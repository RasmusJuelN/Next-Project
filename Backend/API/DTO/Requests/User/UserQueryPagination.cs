namespace API.DTO.Requests.User;

public record class UserQueryPagination
{
    public required int PageSize { get; set; }
    public required string User { get; set; }
    public required Roles Role { get; set; }
    public string? SessionId { get; set; }
}

public enum Roles
{
    Student,
    Teacher
}
namespace Database.DTO.User;

public class MockedUser
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public required UserRoles Role { get; set; }
    public required string Password { get; set; }
}

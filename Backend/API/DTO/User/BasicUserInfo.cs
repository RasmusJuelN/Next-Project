namespace API.DTO.User;

/// <summary>
/// Represents basic user information retrieved from user storage.
/// </summary>
public record class BasicUserInfo
{
    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the groups the user is a member of.
    /// </summary>
    public List<string> MemberOf { get; set; } = [];
}

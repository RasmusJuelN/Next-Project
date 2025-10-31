namespace API.DTO.User;

/// <summary>
/// Represents basic group information retrieved from user storage.
/// </summary>
public record class BasicGroupInfo
{
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
}

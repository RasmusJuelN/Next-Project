
namespace API.DTO.Requests.User;

/// <summary>
/// Represents pagination parameters for querying users with filtering capabilities.
/// </summary>
/// <remarks>
/// This record is used to specify pagination and filtering criteria when retrieving users from the system.
/// It includes page size control, user identification, role-based filtering, and optional session tracking.
/// </remarks>
public record class UserQueryPagination
{
    /// <summary>
    /// Gets or sets the number of items to return per page.
    /// </summary>
    /// <value>
    /// A positive integer representing the maximum number of user records to include in a single page of results.
    /// </value>
    public required int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the user identifier for filtering or context.
    /// </summary>
    /// <value>
    /// A string representing the user identifier used for filtering the query results or providing user context.
    /// </value>
    public required string User { get; set; }

    /// <summary>
    /// Gets or sets the role filter for the user query.
    /// </summary>
    /// <value>
    /// A <see cref="Roles"/> enumeration value used to filter users by their assigned role.
    /// </value>
    public required Roles Role { get; set; }

    /// <summary>
    /// Gets or sets the optional session identifier for tracking purposes.
    /// </summary>
    /// <value>
    /// An optional string representing the session ID. Can be null if session tracking is not required.
    /// </value>
    public string? SessionId { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Roles
{
    Student,
    Teacher
}

namespace API.Interfaces;

/// <summary>
/// Provides methods for authenticating users and searching for authentication system entries using flexible models.
/// </summary>
public interface IAuthenticationBridge
{
    /// <summary>
    /// Authenticates a user with the provided username and password.
    /// </summary>
    /// <param name="username">The username, email, or other unique identifier used by the system to refer to a specific user.</param>
    /// <param name="password">The password used to authenticate the user, in cleartext.</param>
    void Authenticate(string username, string password);

    /// <summary>
    /// Searches for a user using a model whose properties specify the search criteria.
    /// </summary>
    /// <typeparam name="TUser">
    /// The type that maps to a user entry in the authentication system. 
    /// This should be a model or DTO representing a user, with a parameterless constructor.
    /// The properties of this model are used to control which user entries are included in the search.
    /// </typeparam>
    /// <param name="username">The username to search for.</param>
    /// <returns>
    /// Returns an instance of the specified <typeparamref name="TUser"/> type if found, otherwise returns null.
    /// </returns>
    TUser? SearchUser<TUser>(string username) where TUser : new();

    /// <summary>
    /// Searches for a group using a model whose properties specify the search criteria.
    /// </summary>
    /// <typeparam name="TGroup">
    /// The type that maps to a group entry in the authentication system. 
    /// This should be a model or DTO representing a group, with a parameterless constructor.
    /// The properties of this model are used to control which group entries are included in the search.
    /// </typeparam>
    /// <param name="groupName">The name of the group to search for.</param>
    /// <returns>
    /// Returns an instance of the specified <typeparamref name="TGroup"/> type if found, otherwise returns null.
    /// </returns>
    TGroup? SearchGroup<TGroup>(string groupName) where TGroup : new();

    /// <summary>
    /// Searches for an entity using a model whose properties specify the search criteria.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type that maps to an entity entry in the authentication system. 
    /// This should be a model or DTO representing the entity, with a parameterless constructor.
    /// The properties of this model are used to control which entity entries are included in the search.
    /// </typeparam>
    /// <param name="id">The unique identifier of the entity to search for.</param>
    /// <returns>
    /// Returns an instance of the specified type <typeparamref name="TEntity"/> if found, otherwise returns null.
    /// </returns>
    TEntity? SearchId<TEntity>(string id) where TEntity : new();

    /// <summary>
    /// Searches for users matching the provided criteria and returns a single page of results.
    /// </summary>
    /// <typeparam name="TUser">
    /// The user model type to materialize for each result. Must have a public parameterless constructor.
    /// </typeparam>
    /// <param name="username">
    /// The username or partial username to search for. Required.
    /// </param>
    /// <param name="userRole">
    /// Optional role filter; when specified, only users in this role are returned.
    /// </param>
    /// <param name="pageSize">
    /// The maximum number of users to include in the page. Must be a positive integer.
    /// </param>
    /// <param name="sessionId">
    /// Optional paging cursor identifying the current search session or position within the result set.
    /// Pass null to start a new session; use the returned session identifier to request subsequent pages.
    /// </param>
    /// <returns>
    /// <list type="bullet">
    /// <listheader>
    ///     <description>A tuple containing:</description>
    /// </listheader>
    /// <item>
    ///     <term>Item1</term>
    ///     <description>the list of users in the requested page.</description>
    /// </item>
    /// <item>
    ///     <term>Item2</term>
    ///     <description>a session identifier/continuation token for retrieving the next page (null or empty if none).</description>
    /// </item>
    /// <item>
    ///     <term>Item3</term>
    ///     <description>true if additional pages are available; otherwise, false.</description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// To iterate through all results, call this method repeatedly, passing the returned session identifier
    /// until the boolean flag indicates no further pages are available.
    /// </remarks>
    (List<TUser>, string, bool) SearchUserPagination<TUser>(string username, string? userRole, int pageSize, string? sessionId) where TUser : new();

    /// <summary>
    /// Releases all resources used by the implementing object.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Checks if the connection to the authentication system is currently active.
    /// </summary>
    /// <returns>
    /// Returns true if the connection is active, otherwise false.
    /// </returns>
    bool IsConnected();

    /// <summary>
    /// Retrieves an array of entry names to query for the specified mapped entity type.
    /// </summary>
    /// <typeparam name="MappedEntity">
    /// The type of the entity whose properties are to be inspected for <see cref="AuthenticationMapping"/> attributes.
    /// </typeparam>
    /// <returns>
    /// An array of entry names extracted from the <see cref="AuthenticationMapping"/> attributes applied to the properties of <typeparamref name="MappedEntity"/>.
    /// </returns>
    protected static string[] GetEntriesToQuery<MappedEntity>()
    {
        return [.. typeof(MappedEntity).GetProperties()
            .SelectMany(prop => prop.GetCustomAttributes<AuthenticationMapping>())
            .Select(attr => attr.EntryName)];
    }

    /// <summary>
    /// Maps the specified <paramref name="entry"/> object to a new instance of type <typeparamref name="TMappedEntity"/>.
    /// </summary>
    /// <typeparam name="TMappedEntity">
    /// The type to which the <paramref name="entry"/> will be mapped. Must have a parameterless constructor.
    /// </typeparam>
    /// <param name="entry">
    /// The source object to be mapped to the <typeparamref name="TMappedEntity"/> type.
    /// </param>
    /// <returns>
    /// A new instance of <typeparamref name="TMappedEntity"/> with values mapped from <paramref name="entry"/>.
    /// </returns>
    TMappedEntity MapEntry<TMappedEntity>(object entry) where TMappedEntity : new();
}

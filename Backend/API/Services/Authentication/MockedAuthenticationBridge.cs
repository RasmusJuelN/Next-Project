using System.Reflection;
using API.FieldMappers;
using API.Mock;
using Settings.Models;

namespace API.Services.Authentication;

/// <summary>
/// Provides a mocked implementation of the authentication bridge for testing and development purposes.
/// This class simulates authentication using a local JSON file containing user data.
/// </summary>
/// <param name="cacheService">The cache service used for session management during pagination operations.</param>
/// <param name="configuration">The application configuration containing settings for fallback authentication.</param>
/// <remarks>
/// This implementation reads user data from a JSON file located at "./mocked_user_data.json" and provides
/// authentication, user search, group search, and pagination functionality without requiring existing user storage.
/// The class automatically authenticates using service account credentials when performing search operations
/// if not already authenticated.
/// </remarks>
public class MockedAuthenticationBridge(CacheService cacheService, IConfiguration configuration) : BaseAuthenticationBridge(new MockFieldMappingProvider())
{
    private readonly CacheService _CacheService = cacheService;
    private readonly string _MockUserdataFile = "./mocked_user_data.json";
    private bool _Authenticated;
    private readonly LDAPSettings _LdapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);

    public override void Authenticate(string username, string password)
    {
        var users = System.Text.Json.JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText(_MockUserdataFile));
        if (users != null && users.Count != 0 && users.Any(u => u.Username == username && u.Password == password))
        {
            _Authenticated = true;
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
    }

    public override TUser? SearchUser<TUser>(string username) where TUser : default
    {
        EnsureAuthentication();

        List<MockedUser> mockedUsers = System.Text.Json.JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText(_MockUserdataFile)) ?? [];
        MockedUser? mockedUser = mockedUsers.SingleOrDefault(u => u.Username == username);

        if (mockedUser == null)
        {
            return default;
        }

        var user = MapToModel<TUser>(ConvertMockEntryToDictionary(mockedUser));

        return user;
    }

    public override TGroup? SearchGroup<TGroup>(string groupName) where TGroup : default
    {
        EnsureAuthentication();

        List<MockedUser> mockedUsers = System.Text.Json.JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText(_MockUserdataFile)) ?? [];
        string? group = mockedUsers.Where(u => u.Role.ToString().Contains(groupName)).Select(u => u.Role.ToString()).SingleOrDefault();

        if (group == null)
        {
            return default;
        }

        var groupObj = MapToModel<TGroup>(new Dictionary<string, object>
        {
            { "Role", group }
        });

        return groupObj;
    }

    public override TEntity? SearchId<TEntity>(string id) where TEntity : default
    {
        EnsureAuthentication();

        List<MockedUser> mockedUsers = System.Text.Json.JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText(_MockUserdataFile)) ?? [];
        MockedUser? mockedUser = mockedUsers.SingleOrDefault(u => u.Id == Guid.Parse(id));

        if (mockedUser == null)
        {
            return default;
        }

        var user = MapToModel<TEntity>(ConvertMockEntryToDictionary(mockedUser));

        return user;
    }

    public override (List<TMockUser>, string, bool) SearchUserPagination<TMockUser>(string username, string? userRole, int pageSize, string? sessionId) where TMockUser : default
    {
        EnsureAuthentication();

        List<MockedUser> mockedUsers = System.Text.Json.JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText(_MockUserdataFile)) ?? [];

        IEnumerable<MockedUser> filteredUsers = mockedUsers.Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(userRole))
        {
            filteredUsers = filteredUsers.Where(u => u.Role.ToString().Equals(userRole, StringComparison.OrdinalIgnoreCase));
        }

        List<MockedUser> filteredUserList = [.. filteredUsers];

        MockSessionData sessionData;
        if (string.IsNullOrEmpty(sessionId))
        {
            sessionData = new MockSessionData
            {
                FilteredUsers = filteredUserList,
                CurrentIndex = 0
            };
            sessionId = Guid.NewGuid().ToString();
            _CacheService.Set(sessionId, sessionData);
        }
        else
        {
            sessionData = _CacheService.Get<MockSessionData>(sessionId) ?? new MockSessionData
            {
                FilteredUsers = filteredUserList,
                CurrentIndex = 0
            };
        }

        List<TMockUser> pagedUsers = [];

        for (int i = 0; i < pageSize; i++)
        {
            if (sessionData.CurrentIndex + i >= sessionData.FilteredUsers.Count)
            {
                break;
            }

            MockedUser currentUser = sessionData.FilteredUsers[sessionData.CurrentIndex + i];
            TMockUser mappedUser = MapToModel<TMockUser>(ConvertMockEntryToDictionary(currentUser));
            pagedUsers.Add(mappedUser);
        }

        sessionData.CurrentIndex += pagedUsers.Count;
        bool hasMore = sessionData.CurrentIndex < sessionData.FilteredUsers.Count;

        _CacheService.Set(sessionId, sessionData);

        return (pagedUsers, sessionId, hasMore);
    }

    public override void Dispose()
    {
        // No resources to dispose
    }

    public override bool IsConnected() => _Authenticated;

    private static Dictionary<string, object> ConvertMockEntryToDictionary(MockedUser entry)
    {
        var dict = new Dictionary<string, object>();
        foreach (PropertyInfo prop in typeof(MockedUser).GetProperties())
        {
            dict[prop.Name] = prop.GetValue(entry) ?? "";
        }
        return dict;
    }


    private void EnsureAuthentication()
    {
        if (!_Authenticated)
        {
            Authenticate(_LdapSettings.SA, _LdapSettings.SAPassword);
        }
    }
}
using System.Reflection;
using API.Attributes;
using API.DTO.Responses.Auth;
using API.Interfaces;
using API.Utils;
using Database.Enums;

namespace API.Services.Authentication;

public class MockedAuthenticationBridge(CacheService cacheService) : IAuthenticationBridge
{
    private readonly CacheService _CacheService = cacheService;
    private readonly string _MockUserdataFile = "./mocked_user_data.json";
    private bool _Authenticated;

    public void Authenticate(string username, string password)
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

    public TUser? SearchUser<TUser>(string username) where TUser : new()
    {
        if (!_Authenticated)
        {
            throw new UnauthorizedAccessException("Authentication required before searching for users.");
        }

        List<MockedUser> mockedUsers = System.Text.Json.JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText(_MockUserdataFile)) ?? [];
        MockedUser? mockedUser = mockedUsers.SingleOrDefault(u => u.Username == username);

        if (mockedUser == null)
        {
            return default;
        }

        var user = MapEntry<TUser>(mockedUser);

        return user;
    }

    public TGroup? SearchGroup<TGroup>(string groupName) where TGroup : new()
    {
        if (!_Authenticated)
        {
            throw new UnauthorizedAccessException("Authentication required before searching for users.");
        }

        List<MockedUser> mockedUsers = System.Text.Json.JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText(_MockUserdataFile)) ?? [];
        string? group = mockedUsers.Where(u => u.Role.ToString().Contains(groupName)).Select(u => u.Role.ToString()).SingleOrDefault();

        if (group == null)
        {
            return default;
        }

        return (TGroup)(object)group;
    }

    public TEntity? SearchId<TEntity>(string id) where TEntity : new()
    {
        if (!_Authenticated)
        {
            throw new UnauthorizedAccessException("Authentication required before searching for users.");
        }

        List<MockedUser> mockedUsers = System.Text.Json.JsonSerializer.Deserialize<List<MockedUser>>(File.ReadAllText(_MockUserdataFile)) ?? [];
        MockedUser? mockedUser = mockedUsers.SingleOrDefault(u => u.objectGUID == Guid.Parse(id));

        if (mockedUser == null)
        {
            return default;
        }

        JWTUser jwtUser = new()
        {
            Guid = mockedUser.objectGUID,
            Username = mockedUser.Username,
            Name = mockedUser.Name,
            Role = mockedUser.Role.ToString(),
            Permissions = (int)mockedUser.Role
        };

        return (TEntity)(object)jwtUser;
    }

    public (List<TMockUser>, string, bool) SearchUserPagination<TMockUser>(string username, string? userRole, int pageSize, string? sessionId) where TMockUser : new()
    {
        if (!_Authenticated)
        {
            throw new UnauthorizedAccessException("Authentication required before searching for users.");
        }

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
        
        List<TMockUser> pagedUsers = [.. sessionData.FilteredUsers
                .Skip(sessionData.CurrentIndex)
                .Take(pageSize)
                .Select(u => (TMockUser)(object)new JWTUser
                {
                    Guid = u.objectGUID,
                    Username = u.Username,
                    Name = u.Name,
                    Role = u.Role.ToString(),
                    Permissions = (int)u.Role
                })];

        sessionData.CurrentIndex += pagedUsers.Count;
        bool hasMore = sessionData.CurrentIndex < sessionData.FilteredUsers.Count;

        _CacheService.Set(sessionId, sessionData);

        return (pagedUsers, sessionId, hasMore);
    }

    public MappedEntity MapEntry<MappedEntity>(object entry) where MappedEntity : new()
    {
        if (entry is not MockedUser mockedUserEntry)
        {
            throw new ArgumentException("The provided entry must be of type MockedUser.");
        }
        
        MappedEntity mappedEntity = new();

        foreach (PropertyInfo prop in typeof(MappedEntity).GetProperties())
        {
            foreach (AuthenticationMapping attr in prop.GetCustomAttributes<AuthenticationMapping>())
            {
                prop.SetValue(mappedEntity, mockedUserEntry.GetType().GetProperty(attr.EntryName)?.GetValue(mockedUserEntry));
            }
        }

        return mappedEntity;
    }

    public void Dispose()
    {
        // No resources to dispose
    }

    public bool IsConnected() => _Authenticated;
}

public class MockSessionData
{
    public List<MockedUser> FilteredUsers { get; set; } = [];
    public int CurrentIndex { get; set; } = 0;
}
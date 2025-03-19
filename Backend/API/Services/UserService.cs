using API.DTO.LDAP;
using API.DTO.Requests.User;
using API.DTO.Responses.User;
using API.Extensions;

namespace API.Services;

public class UserService(LdapService ldapService)
{
    private readonly LdapService _ldapService = ldapService;

    public UserQueryPaginationResult QueryLDAPUsersWithPagination(UserQueryPagination request)
    {
        (List<BasicUserInfoWithObjectGuid> ldapUsers, string sessionId, bool hasMore) = _ldapService.SearchUserPagination<BasicUserInfoWithObjectGuid>(request.User, request.Role.ToString(), request.PageSize, request.SessionId);

        return new()
        {
            UserBases = [.. ldapUsers.Select(u => u.ToUserBaseDto())],
            SessionId = sessionId,
            HasMore = hasMore
        };
    }
}

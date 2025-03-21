using API.DTO.LDAP;
using API.DTO.Requests.User;
using API.DTO.Responses.User;
using API.Extensions;
using API.Interfaces;
using Database.DTO.ActiveQuestionnaire;

namespace API.Services;

public class UserService(LdapService ldapService, IUnitOfWork unitOfWork)
{
    private readonly LdapService _ldapService = ldapService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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

    public async Task<List<UserSpecificActiveQuestionnaireBase>> GetActiveQuestionnaires(Guid userId)
    {
        return await _unitOfWork.User.GetAllAssociatedActiveQuestionnaires(userId);
    }
}

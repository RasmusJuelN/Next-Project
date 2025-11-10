using API.DTO.Requests.ActiveQuestionnaire;
using API.DTO.Requests.User;
using API.DTO.Responses.ActiveQuestionnaire;
using API.DTO.Responses.User;
using Database.DTO.ActiveQuestionnaire;

public interface IUserService
{
    Task<ActiveQuestionnaireKeysetPaginationResultStudent> GetActiveQuestionnairesForStudent(
        ActiveQuestionnaireKeysetPaginationRequestStudent request, Guid userId);

    Task<ActiveQuestionnaireKeysetPaginationResultTeacher> GetActiveQuestionnairesForTeacher(
        ActiveQuestionnaireKeysetPaginationRequestTeacher request, Guid userId);

    Task<List<ActiveQuestionnaireBase>> GetPendingActiveQuestionnaires(Guid userId);

    UserQueryPaginationResult QueryLDAPUsersWithPagination(UserQueryPagination request);
}

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<ActiveQuestionnaireKeysetPaginationResultStudent> GetActiveQuestionnairesForStudent(
            ActiveQuestionnaireKeysetPaginationRequestStudent request, Guid userId);

        Task<ActiveQuestionnaireKeysetPaginationResultTeacher> GetActiveQuestionnairesForTeacher(
            ActiveQuestionnaireKeysetPaginationRequestTeacher request, Guid userId);

        Task<List<ActiveQuestionnaireBase>> GetPendingActiveQuestionnaires(Guid userId);

        UserQueryPaginationResult QueryLDAPUsersWithPagination(UserQueryPagination request);

        Task<QuestionnaireGroupOffsetPaginationResult> FetchActiveQuestionnaireGroupsForTeacherWithOffsetPagination(QuestionnaireGroupOffsetPaginationRequest request, Guid teacherGuid);

        Task<List<LdapUserBase>> SearchStudentsRelatedToTeacherAsync(Guid teacherId, string studentUsernameQuery);
    }
}


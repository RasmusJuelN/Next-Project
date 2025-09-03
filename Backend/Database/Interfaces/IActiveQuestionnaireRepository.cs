using Database.DTO.ActiveQuestionnaire;
using Database.Enums;

namespace Database.Interfaces;

public interface IActiveQuestionnaireRepository
{
    Task<ActiveQuestionnaireBase> GetActiveQuestionnaireBaseAsync(Guid id);
    Task<ActiveQuestionnaire> GetFullActiveQuestionnaireAsync(Guid id);
    Task<ActiveQuestionnaire> ActivateQuestionnaireAsync(
        Guid questionnaireTemplateId,
        Guid studentId,
        Guid teacherId,
        Guid groupId);
    Task<(List<ActiveQuestionnaireBase>, int)> PaginationQueryWithKeyset(
        int amount,
        ActiveQuestionnaireOrderingOptions sortOrder,
        Guid? cursorIdPosition = null,
        DateTime? cursorActivatedAtPosition = null,
        string? titleQuery = null,
        string? student = null,
        string? teacher = null,
        Guid? idQuery = null,
        Guid? userId = null,
        bool onlyStudentCompleted = false,
        bool onlyTeacherCompleted = false);
    Task AddAnswers(Guid activeQuestionnaireId, Guid userId, AnswerSubmission submission);
    Task<bool> HasUserSubmittedAnswer(Guid userId, Guid activeQuestionnaireId);
    Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId);
    Task<bool> IsActiveQuestionnaireComplete(Guid activeQuestionnaireId, Guid userId);
    Task<FullResponse> GetFullResponseAsync(Guid id);
    Task<List<ActiveQuestionnaireBase>> GetPendingActiveQuestionnaires(Guid id);

    Task<List<FullResponse>> GetResponsesFromStudentAndTemplateAsync(Guid studentid, Guid templateid);
}

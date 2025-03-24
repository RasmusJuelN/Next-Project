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
        Guid teacherId);
    Task<(List<ActiveQuestionnaireBase>, int)> PaginationQueryWithKeyset(
        int amount,
        Guid? cursorIdPosition,
        DateTime? cursorActivatedAtPosition,
        ActiveQuestionnaireOrderingOptions sortOrder,
        string? titleQuery,
        string? student,
        string? teacher,
        Guid? idQuery);
    Task AddAnswers(Guid activeQuestionnaireId, Guid userId, AnswerSubmission submission);
    Task<bool> HasUserSubmittedAnswer(Guid userId, Guid activeQuestionnaireId);
    Task<FullResponse> GetFullResponseAsync(Guid id);
}

using Database.DTO.ActiveQuestionnaire;

namespace API.DTO.Responses.ActiveQuestionnaire;

public record class ActiveQuestionnaireKeysetPaginationResultBase
{
    public string? QueryCursor { get; set; }
    public int TotalCount { get; set; }
}

public record class ActiveQuestionnaireKeysetPaginationResultStudent : ActiveQuestionnaireKeysetPaginationResultBase
{
    public required List<ActiveQuestionnaireStudentBase> ActiveQuestionnaireBases { get; set; }
}

public record class ActiveQuestionnaireKeysetPaginationResultTeacher : ActiveQuestionnaireKeysetPaginationResultBase
{
    public required List<ActiveQuestionnaireTeacherBase> ActiveQuestionnaireBases { get; set; }
}

public record class ActiveQuestionnaireKeysetPaginationResultAdmin : ActiveQuestionnaireKeysetPaginationResultBase
{
    public required List<ActiveQuestionnaireBase> ActiveQuestionnaireBases { get; set; }
}

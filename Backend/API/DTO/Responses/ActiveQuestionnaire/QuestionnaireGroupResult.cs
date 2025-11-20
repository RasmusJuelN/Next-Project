

using Database.Enums;

namespace API.DTO.Responses.ActiveQuestionnaire
{
    /// <summary>
    /// Represents a basic result containing essential information about a questionnaire group.
    /// </summary>
    /// <remarks>
    /// This class is used as a data transfer object (DTO) to provide minimal group information
    /// in API responses, containing only the group identifier and name.
    /// </remarks>
    public class QuestionnaireGroupBasicResult
    {
        public Guid GroupId { get; set; }
        required public string Name { get; set; }
    }

    /// <summary>
    /// Represents a detailed data transfer object (DTO) for a questionnaire group,
    /// including its identifiers, name, associated template, and a collection of
    /// active questionnaires with administrative details.
    /// </summary>
    /// <remarks>
    /// This DTO is returned by API endpoints when full questionnaire information
    /// is required, including student and teacher completion states.
    /// </remarks>
    public class QuestionnaireGroupResult : QuestionnaireGroupBasicResult
    {
        public Guid TemplateId { get; set; }
        public List<ActiveQuestionnaireAdminBase> Questionnaires { get; set; }
        public required ActiveQuestionnaireType QuestionnaireType { get; set; }
    }
}

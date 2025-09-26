

namespace API.DTO.Responses.ActiveQuestionnaire
{
    /// <summary>
    /// Represents a detailed data transfer object (DTO) for a questionnaire group,
    /// including its identifiers, name, associated template, and a collection of
    /// active questionnaires with administrative details.
    /// </summary>
    /// <remarks>
    /// This DTO is returned by API endpoints when full questionnaire information
    /// is required, including student and teacher completion states.
    /// </remarks>
    public class QuestionnaireGroupResult
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public Guid TemplateId { get; set; }
        public List<ActiveQuestionnaireAdminBase> Questionnaires { get; set; }
    }
}

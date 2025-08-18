

namespace API.DTO.Responses.ActiveQuestionnaire
{
    public class QuestionnaireGroupResult
{
    public Guid GroupId { get; set; }
    public string Name { get; set; }
    public Guid TemplateId { get; set; }
    public List<ActiveQuestionnaireAdminBase> Questionnaires { get; set; }
}
}

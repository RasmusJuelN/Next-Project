namespace API.DTO.Requests.ActiveQuestionnaire
{
    public class ActivateQuestionnaireGroup
    {
        public string Name { get; set; }
        public Guid TemplateId { get; set; }
        public List<Guid> StudentIds { get; set; }
        public List<Guid> TeacherIds { get; set; }
    }
}

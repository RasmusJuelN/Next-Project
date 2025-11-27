namespace API.DTO.Requests.ActiveQuestionnaire
{
    /// <summary>
    /// Represents the request payload for creating and activating a new questionnaire group
    /// from a specific template and assigning participants.
    /// </summary>
    /// <remarks>
    /// This request is typically sent by clients when initiating a new group of questionnaires,
    /// specifying the template, group name, and the participating students and teachers.
    /// </remarks>
    public class ActivateQuestionnaireGroup
    {
        /// <summary>
        /// Gets or sets the display name of the new questionnaire group.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the questionnaire template to activate.
        /// </summary>
        public required Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the list of GUIDs representing the student participants.
        /// </summary>
        public List<Guid> StudentIds { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of GUIDs representing the teacher participants.
        /// </summary>
        public List<Guid> TeacherIds { get; set; } = [];

        /// <summary>
        /// Gets or sets the type of the active questionnaire to be created.
        /// </summary>
        public required ActiveQuestionnaireType QuestionnaireType { get; set; }
    }
}

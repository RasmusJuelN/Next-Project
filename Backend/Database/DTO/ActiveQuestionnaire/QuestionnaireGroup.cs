using Database.Models;

namespace Database.DTO.ActiveQuestionnaire
{
    /// <summary>
    /// Represents a full data transfer object (DTO) for a questionnaire group,
    /// extending <see cref="QuestionnaireGroupBase"/> with full questionnaire details.
    /// </summary>
    /// <remarks>
    /// This DTO is used when complete questionnaire information is needed,
    /// such as in detailed views or administrative tools.
    /// </remarks>
    public record class QuestionnaireGroup : QuestionnaireGroupBase
    {
        /// <summary>
        /// Gets or sets the collection of active questionnaires associated with the group.
        /// </summary>
        public List<StandardActiveQuestionnaireModel> Questionnaires { get; set; } = new();
    }
}


namespace Database.Models
{
    /// <summary>
    /// Represents the database entity for a questionnaire group, including its identifying
    /// information, associated template, and collection of active questionnaires.
    /// </summary>
    /// <remarks>
    /// This model is used by Entity Framework Core to map questionnaire group data to the database.
    /// It defines relationships to both the <see cref="QuestionnaireTemplateModel"/> and the
    /// collection of <see cref="ActiveQuestionnaireModel"/> entities.
    /// </remarks>
    public class QuestionnaireGroupModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the questionnaire group.
        /// </summary>
        /// <remarks>
        /// Serves as the primary key in the database table.
        /// </remarks>
        [Key]
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated questionnaire template.
        /// </summary>
        /// <remarks>
        /// Represents a foreign key to the <see cref="QuestionnaireTemplateModel"/>.
        /// </remarks>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the questionnaire group.
        /// </summary>
        /// <remarks>
        /// Typically used for identifying and grouping related questionnaires in the UI.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the group was created.
        /// </summary>
        /// <remarks>
        /// Useful for sorting, auditing, and pagination operations.
        /// </remarks>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the questionnaire template associated with this group.
        /// </summary>
        /// <remarks>
        /// This navigation property provides access to the template details from which
        /// the questionnaires in the group were derived.
        /// </remarks>
        public QuestionnaireTemplateModel Template { get; set; }

        /// <summary>
        /// Gets or sets the collection of active questionnaires belonging to this group.
        /// </summary>
        /// <remarks>
        /// This navigation property represents the one-to-many relationship between
        /// the group and its questionnaires.
        /// </remarks>
        public ICollection<ActiveQuestionnaireModel> Questionnaires { get; set; }
    }

}

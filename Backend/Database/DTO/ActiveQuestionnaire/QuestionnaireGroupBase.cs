using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.DTO.ActiveQuestionnaire
{

    /// <summary>
    /// Represents a lightweight data transfer object (DTO) for a questionnaire group,
    /// providing essential details without full questionnaire navigation data.
    /// </summary>
    /// <remarks>
    /// This DTO is useful for list views, summaries, or pagination queries where
    /// only the core group information is required.
    /// </remarks>
    public record class QuestionnaireGroupBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the questionnaire group.
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated template.
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the questionnaire group.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the group was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the title of the associated template, if available.
        /// </summary>
        public string? TemplateTitle { get; set; }

        /// <summary>
        /// Gets or sets the total number of questionnaires in the group.
        /// </summary>
        public int QuestionnaireCount { get; set; }
    }
}

﻿using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<ActiveQuestionnaireModel> Questionnaires { get; set; } = new();
    }
}

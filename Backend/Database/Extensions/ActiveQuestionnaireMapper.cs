
namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping StandardActiveQuestionnaireModel entities to various DTO representations.
/// This class handles the conversion between database models and data transfer objects for active questionnaires.
/// </summary>
public static class ActiveQuestionnaireMapper
{
    /// <summary>
    /// Converts an StandardActiveQuestionnaireModel to an ActiveQuestionnaireBase DTO containing basic questionnaire information.
    /// </summary>
    /// <param name="activeQuestionnaire">The StandardActiveQuestionnaireModel entity to convert.</param>
    /// <returns>
    /// An ActiveQuestionnaireBase DTO containing essential questionnaire data including ID, title, description,
    /// activation timestamp, and basic student and teacher information.
    /// </returns>
    /// <remarks>
    /// This method creates a lightweight representation of the active questionnaire suitable for list views
    /// and scenarios where detailed question data is not required. It includes completion timestamps for
    /// both student and teacher participants.
    /// </remarks>
    public static ActiveQuestionnaireBase ToBaseDto(this StandardActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            GroupId = activeQuestionnaire.GroupId,
            TemplateId = activeQuestionnaire.QuestionnaireTemplateFK,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student.ToBaseDto(),
            Teacher = activeQuestionnaire.Teacher.ToBaseDto(),
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt,
            QuestionnaireType = activeQuestionnaire.QuestionnaireType
        };
    }

    /// <summary>
    /// Converts an StandardActiveQuestionnaireModel to a complete ActiveQuestionnaire DTO including all questions.
    /// </summary>
    /// <param name="activeQuestionnaire">The StandardActiveQuestionnaireModel entity to convert.</param>
    /// <returns>
    /// An ActiveQuestionnaire DTO containing complete questionnaire data including ID, title, description,
    /// activation timestamp, student and teacher information, completion timestamps, and all associated questions.
    /// </returns>
    /// <remarks>
    /// This method creates a comprehensive representation of the active questionnaire that includes the full
    /// question hierarchy from the associated questionnaire template. Use this method when you need access
    /// to the complete questionnaire structure for display or processing purposes.
    /// </remarks>
    public static ActiveQuestionnaire ToDto(this StandardActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            GroupId = activeQuestionnaire.GroupId,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student.ToBaseDto(),
            Teacher = activeQuestionnaire.Teacher.ToBaseDto(),
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt,
            QuestionnaireType = activeQuestionnaire.QuestionnaireType,
            Questions = [.. activeQuestionnaire.QuestionnaireTemplate.Questions.Select(q => q.ToDto())]
        };
    }

    /// <summary>
    /// Converts an StandardActiveQuestionnaireModel to a FullResponse DTO containing complete questionnaire data with answers.
    /// </summary>
    /// <param name="activeQuestionnaire">The StandardActiveQuestionnaireModel entity to convert, must include student and teacher answers.</param>
    /// <returns>
    /// A FullResponse DTO containing the questionnaire metadata, participant information with completion timestamps,
    /// and a comprehensive list of question-answer pairs from both student and teacher perspectives.
    /// </returns>
    /// <remarks>
    /// This method creates the most comprehensive representation of a completed questionnaire, including all
    /// submitted answers from both participants. It pairs student and teacher answers for each question,
    /// handling both custom text responses and selected option responses. This method should only be used
    /// for questionnaires where both participants have completed their responses.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the activeQuestionnaire parameter is null or when required completion timestamps are null.
    /// </exception>
    public static FullResponse ToFullResponseAll(this StandardActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            Student = new() { User = activeQuestionnaire.Student.ToDto(), CompletedAt = (DateTime)activeQuestionnaire.StudentCompletedAt! },
            Teacher = new() { User = activeQuestionnaire.Teacher.ToDto(), CompletedAt = (DateTime)activeQuestionnaire.TeacherCompletedAt! },
            Answers = [.. activeQuestionnaire.StudentAnswers.Zip(activeQuestionnaire.TeacherAnswers).Select(a => new FullAnswer {
                Question = a.First.Question!.Prompt,
                StudentResponse = a.First.CustomResponse ?? a.First.Option!.DisplayText,
                IsStudentResponseCustom = a.First.CustomResponse is not null,
                TeacherResponse = a.Second.CustomResponse ?? a.Second.Option!.DisplayText,
                IsTeacherResponseCustom = a.Second.CustomResponse is not null,
                Options = a.First.Question!.Options?.Select(option => new QuestionOption
                {
                    DisplayText = option.DisplayText,
                    OptionValue = option.OptionValue.ToString(),
                    IsSelectedByStudent = a.First.Option?.Id == option.Id,
                    IsSelectedByTeacher = a.Second.Option?.Id == option.Id
                }).ToList()
            })]
        };
    }
    public static FullStudentRespondsDate ToFullStudentRespondsDate(this StandardActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            Student = new() { User = activeQuestionnaire.Student.ToDto(), CompletedAt = (DateTime)activeQuestionnaire.StudentCompletedAt! },
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            Answers = [.. activeQuestionnaire.StudentAnswers.Select(a => new StudentAnswer {
                Question = a.Question!.Prompt,
                StudentResponse = a.CustomResponse ?? a.Option!.DisplayText,
                IsStudentResponseCustom = a.CustomResponse is not null
            })]
        };
    }

}

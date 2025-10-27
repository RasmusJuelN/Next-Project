using Database.DTO.User;
using Database.DTO.QuestionnaireTemplate;

namespace Database.DTO.ActiveQuestionnaire;

/// <summary>
/// Represents the base response data transfer object for questionnaire responses.
/// </summary>
/// <remarks>
/// This record class serves as a foundation for response-related DTOs in the active questionnaire system.
/// It contains the essential properties that identify and describe a response.
/// </remarks>
public record class ResponseBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Represents a student's response to an active questionnaire, containing the student information and their answers.
/// </summary>
/// <remarks>
/// This record inherits from ResponseBase and encapsulates all data related to a single student's 
/// participation in a questionnaire, including their personal information and complete set of answers.
/// </remarks>
public record class StudentResponse : ResponseBase
{
    public required Student Student { get; set; }
    public required List<StudentAnswer> Answers { get; set; }
}

/// <summary>
/// Represents a teacher's response to an active questionnaire, containing the student being evaluated,
/// the teacher providing the response, and the collection of answers provided by the teacher.
/// </summary>
/// <remarks>
/// This record inherits from ResponseBase and is used to capture teacher evaluations or feedback
/// about specific students through structured questionnaire responses.
/// </remarks>
public record class TeacherResponse : ResponseBase
{
    public required Student Student { get; set; }
    public required Teacher Teacher { get; set; }
    public required List<TeacherAnswer> Answers { get; set; }
}



/// <summary>
/// Represents a complete questionnaire response containing all associated data including student, teacher, and answers.
/// </summary>
/// <remarks>
/// This record extends <see cref="ResponseBase"/> and provides a comprehensive view of a questionnaire response
/// with fully populated related entities. It includes the responding student, the associated teacher,
/// and a complete list of answers provided in the response.
/// </remarks>
public record class FullResponse : ResponseBase
{
    public required Student Student { get; set; }
    public required Teacher Teacher { get; set; }
    public required List<FullAnswer> Answers { get; set; }
}

public record class FullStudentRespondsDate : StudentResponse
{
    public DateTime? StudentCompletedAt { get; set; }
}

//#################################################//

/// <summary>
/// Represents a student who has completed a questionnaire.
/// </summary>
/// <remarks>
/// This record contains information about a student's questionnaire completion,
/// including their user details and the timestamp when they finished.
/// </remarks>
public record class Student
{
    public required FullUser User { get; set; }
    public required DateTime CompletedAt { get; set; }
}

/// <summary>
/// Represents a teacher who has completed a questionnaire.
/// </summary>
/// <remarks>
/// This record contains information about a teacher's questionnaire completion,
/// including their user details and the timestamp when they finished.
/// </remarks>
public record class Teacher
{
    public required FullUser User { get; set; }
    public required DateTime CompletedAt { get; set; }
}

/// <summary>
/// Represents the base structure for an answer in a questionnaire response.
/// </summary>
/// <remarks>
/// This record serves as a foundation for answer types and contains the essential
/// question information that all answers must reference.
/// </remarks>
public record AnswerBase
{
    public required string Question { get; set; }
}

/// <summary>
/// Represents a student's answer to a questionnaire question.
/// </summary>
/// <remarks>
/// This record extends <see cref="AnswerBase"/> and contains the student's response data
/// along with metadata indicating whether the response was a custom input.
/// </remarks>
public record StudentAnswer : AnswerBase
{
    public required string StudentResponse { get; set; }
    public required bool IsStudentResponseCustom { get; set; }
}

/// <summary>
/// Represents a teacher's answer to a questionnaire question.
/// </summary>
/// <remarks>
/// This record extends <see cref="StudentAnswer"/> and contains the teacher's response data
/// along with metadata indicating whether the response was a custom input.
/// </remarks>
public record TeacherAnswer : StudentAnswer
{
    public required string TeacherResponse { get; set; }
    public required bool IsTeacherResponseCustom { get; set; }
}

/// <summary>
/// Represents a question option with selection status for both student and teacher.
/// </summary>
/// <remarks>
/// This record contains the display text and value of a question option,
/// along with boolean flags indicating whether it was selected by the student and/or teacher.
/// </remarks>
public record QuestionOption
{
    public required string DisplayText { get; set; }
    public required string OptionValue { get; set; }
    public required bool IsSelectedByStudent { get; set; }
    public required bool IsSelectedByTeacher { get; set; }
}

/// <summary>
/// Represents a complete answer in an active questionnaire, containing all answer details.
/// Inherits from TeacherAnswer to provide full answer functionality.
/// </summary>
public record FullAnswer : TeacherAnswer 
{
    /// <summary>
    /// Gets or sets the collection of question options with selection status.
    /// This property contains all available options for the question along with
    /// indicators of which options were selected by the student and teacher.
    /// </summary>
    public List<QuestionOption>? Options { get; set; }
};




    public class StudentResultHistory
    {
        public required UserBase Student { get; set; }
        public required UserBase Teacher { get; set; }
        public required QuestionnaireTemplate.QuestionnaireTemplate Template { get; set; }
        public required List<AnswerInfo> AnswersInfo { get; set; }
    }

    public class AnswerInfo
    {
        public required DateTime? StudentCompletedAt { get; set; }
        public required DateTime? TeacherCompletedAt { get; set; }
        public required List<AnswerDetails> Answers { get; set; }
    }

    public class AnswerDetails
    {
        public required string QuestionId { get; set; }
        public required string? StudentResponse { get; set; }
        public required bool? IsStudentResponseCustom { get; set; }
        public required List<int>? SelectedOptionIdsByStudent { get; set; }
        public required string? TeacherResponse { get; set; }
        public required bool? IsTeacherResponseCustom { get; set; }
        public required List<int>? SelectedOptionIdsByTeacher { get; set; }
    }
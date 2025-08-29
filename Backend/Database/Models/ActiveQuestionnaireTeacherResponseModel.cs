namespace Database.Models;

/// <summary>
/// Represents a response, assessment, or feedback submitted by a teacher for an active questionnaire.
/// Inherits all response properties from the base model and serves as the teacher-specific discriminator in the TPH inheritance pattern.
/// </summary>
/// <remarks>
/// This model extends ActiveQuestionnaireResponseBaseModel to represent teacher-specific responses.
/// Uses Table Per Hierarchy (TPH) inheritance where Entity Framework adds a discriminator column
/// to distinguish between student and teacher responses in the same table.
/// Teacher responses typically represent assessments, feedback, or evaluations of student submissions.
/// </remarks>
public class ActiveQuestionnaireTeacherResponseModel : ActiveQuestionnaireResponseBaseModel
{

}

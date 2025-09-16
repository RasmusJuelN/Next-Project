namespace Database.Models;

/// <summary>
/// Represents a response submitted by a student to an active questionnaire.
/// Inherits all response properties from the base model and serves as the student-specific discriminator in the TPH inheritance pattern.
/// </summary>
/// <remarks>
/// This model extends ActiveQuestionnaireResponseBaseModel to represent student-specific responses.
/// Uses Table Per Hierarchy (TPH) inheritance where Entity Framework adds a discriminator column
/// to distinguish between student and teacher responses in the same table.
/// </remarks>
public class ActiveQuestionnaireStudentResponseModel : ActiveQuestionnaireResponseBaseModel
{

}

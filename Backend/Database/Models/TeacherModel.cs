namespace Database.Models;

/// <summary>
/// Represents a teacher user in the system, inheriting all base user properties and behaviors.
/// </summary>
/// <remarks>
/// This model extends UserBaseModel to represent teacher-specific users in the application.
/// Teachers participate in questionnaires by providing responses to assigned questionnaires
/// and can have multiple active questionnaires simultaneously with different teachers.
/// Uses Table Per Hierarchy (TPH) inheritance pattern where teacher-specific data is stored
/// in the same table as the base user properties, differentiated by a discriminator column.
/// </remarks>
public class TeacherModel : UserBaseModel {}


namespace Database.Enums;

/// <summary>
/// Represents the different roles that a user can have in the system.
/// This enum is serialized as string values in JSON format.
/// </summary>
/// <remarks>
/// The enum uses JsonStringEnumConverter to ensure that when serialized to JSON,
/// the enum values appear as their string representations rather than numeric values.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRoles
{
    Student,
    Teacher,
    Admin
}

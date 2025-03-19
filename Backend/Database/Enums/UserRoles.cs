using System.Text.Json.Serialization;

namespace Database.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRoles
{
    Student,
    Teacher,
    Admin
}

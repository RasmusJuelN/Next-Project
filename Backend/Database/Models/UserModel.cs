using Database.Enums;

namespace Database.Models;

internal class UserModel
{
    internal int Id { get; set; }
    internal required string UserName { get; set; }
    internal required string FullName { get; set; }
    internal required UserRoles PrimaryRole { get; set; }
    internal required UserPermissions Permissions { get; set; }

    // TODO: Change to reflect new database model
    // Navigational properties and references
    internal required ICollection<ActiveQuestionnaire> ?StudentQuestionnaires { get; set; }
    internal required ICollection<ActiveQuestionnaire> ?TeacherQuestionnaires { get; set; }
}

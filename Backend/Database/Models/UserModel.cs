using Database.Enums;

namespace Database.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public required UserRoles PrimaryRole { get; set; }
    public required UserPermissions Permissions { get; set; }

    // Navigational properties and references
    public ICollection<ActiveQuestionnaireModel>? ActiveQuestionnaires { get; set; }
}

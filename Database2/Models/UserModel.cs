using Database.Enums;

namespace Database.Models;

internal class UserModel
{
    internal int Id { get; set; }
    internal required string UserName { get; set; }
    internal required string FullName { get; set; }
    internal required UserRoles Role { get; set; }

    // Navigational properties and references
    internal required ICollection<ActiveQuestionnaireModel> ActiveQuestionnaires { get; set; }
}

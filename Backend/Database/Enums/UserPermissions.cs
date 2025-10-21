namespace Database.Enums;

/// <summary>
/// Defines the permissions available to users in the application.
/// This enum uses the Flags attribute to allow bitwise combination of multiple permissions.
/// </summary>
/// <remarks>
/// The enum includes individual permissions as well as predefined role presets:
/// <list type="bullet">
/// <item><description>Individual permissions use power-of-2 values for bitwise operations</description></item>
/// <item><description>Preset roles combine multiple permissions for common user types</description></item>
/// </list>
/// </remarks>
[Flags]
public enum UserPermissions
{
    None = 0,
    CanModifyQuestionnaireTemplates = 1,
    CanModifyApplicationSettings = 2,
    CanViewApplicationLogs = 4,
    CanAssignQuestionnaires = 8,
    CanRespondToQuestionnaires = 16,
    CanModifyPermissions = 32,

    // Presets
    Student = CanRespondToQuestionnaires,
    Teacher = CanRespondToQuestionnaires | CanAssignQuestionnaires,
    Admin = CanAssignQuestionnaires | CanModifyQuestionnaireTemplates | CanModifyApplicationSettings | CanViewApplicationLogs,
    SuperAdmin = Admin | CanModifyPermissions
}

namespace Database.Enums;

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
    Admin = CanModifyQuestionnaireTemplates | CanModifyApplicationSettings | CanViewApplicationLogs,
    SuperAdmin = Admin | CanModifyPermissions
}

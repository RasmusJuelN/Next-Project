using Database.Enums;

namespace Database.DTO.User;

public record class FullUser : UserBase
{
    public required Guid Guid { get; set; }
    public required UserRoles PrimaryRole { get; set; }
    public required UserPermissions Permissions { get; set; }

    //// added to see classname 
    //public string? ClassName { get; set; }
}

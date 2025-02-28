using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[Table("User")]
[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(Guid), IsUnique = true)]
public class UserBaseModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required Guid Guid { get; set; }

    [MaxLength(100)]
    [Required]
    public required string UserName { get; set; }
    
    [MaxLength(100)]
    [Required]
    public required string FullName { get; set; }
    
    [Required]
    public required UserRoles PrimaryRole { get; set; }
    
    [Required]
    public required UserPermissions Permissions { get; set; }

    // Navigational properties and references
    public virtual ICollection<ActiveQuestionnaireModel> ActiveQuestionnaires { get; set; } = [];

    public virtual ICollection<TrackedRefreshTokenModel> TrackedRefreshTokens { get; set; } = [];
}

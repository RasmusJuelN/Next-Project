using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

/// <summary>
/// Represents the base user model containing common properties shared by all user types in the system.
/// Serves as the foundation for student and teacher models using Table Per Hierarchy (TPH) inheritance.
/// </summary>
/// <remarks>
/// This base model contains essential user information including identity, authentication, and authorization data.
/// Derived models (StudentModel, TeacherModel) extend this base to provide role-specific functionality.
/// The model integrates with LDAP/Active Directory through the Guid field and supports comprehensive
/// role-based access control through PrimaryRole and Permissions properties.
/// Multiple unique indexes ensure data integrity for critical identification fields.
/// </remarks>
[Table("User")]
[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(Guid), IsUnique = true)]
public class UserBaseModel
{
    /// <summary>
    /// Gets or sets the unique database identifier for this user.
    /// </summary>
    /// <remarks>
    /// Internal database primary key used for foreign key relationships and efficient joins.
    /// </remarks>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the globally unique identifier from the external identity provider (LDAP/Active Directory).
    /// </summary>
    /// <remarks>
    /// This GUID typically comes from Active Directory and provides the authoritative user identity
    /// across systems. Must be unique across all users and is indexed for efficient lookup.
    /// Used for authentication integration and cross-system user correlation.
    /// </remarks>
    [Required]
    public required Guid Guid { get; set; }

    /// <summary>
    /// Gets or sets the unique username for this user.
    /// </summary>
    /// <remarks>
    /// Maximum length of 100 characters. Must be unique across all users and is indexed.
    /// Typically corresponds to the user's login name in the external identity system.
    /// Used for user identification in UI and logging contexts.
    /// </remarks>
    [MaxLength(100)]
    [Required]
    public required string UserName { get; set; }
    
    /// <summary>
    /// Gets or sets the full display name of the user.
    /// </summary>
    /// <remarks>
    /// Maximum length of 100 characters. Contains the user's complete name for display purposes.
    /// Used in user interfaces, reports, and communication where a human-readable name is needed.
    /// </remarks>
    [MaxLength(100)]
    [Required]
    public required string FullName { get; set; }
    
    /// <summary>
    /// Gets or sets the primary role assigned to this user in the system.
    /// </summary>
    /// <remarks>
    /// Determines the user's primary function and access level within the application.
    /// Used for role-based authorization and UI customization. Complemented by the
    /// more granular Permissions property for fine-grained access control.
    /// </remarks>
    [Required]
    public required UserRoles PrimaryRole { get; set; }
    
    /// <summary>
    /// Gets or sets the detailed permissions granted to this user.
    /// </summary>
    /// <remarks>
    /// Provides fine-grained access control beyond the basic role assignment.
    /// Used for feature-level authorization and determining specific user capabilities
    /// within the application. Can be used to grant or restrict access to specific features.
    /// </remarks>
    [Required]
    public required UserPermissions Permissions { get; set; }

    /// <summary>
    /// Gets or sets the collection of active questionnaires associated with this user.
    /// </summary>
    /// <remarks>
    /// Contains questionnaires where this user participates either as a student or teacher.
    /// Virtual property enables lazy loading of questionnaire associations when needed.
    /// Used for dashboard displays and workflow management.
    /// </remarks>
    public virtual ICollection<StandardActiveQuestionnaireModel> ActiveQuestionnaires { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of refresh tokens issued to this user for authentication.
    /// </summary>
    /// <remarks>
    /// Contains all refresh tokens (both active and revoked) associated with this user's sessions.
    /// Used for session management, token rotation, and security policies.
    /// Virtual property enables lazy loading of token information when needed.
    /// </remarks>
    public virtual ICollection<TrackedRefreshTokenModel> TrackedRefreshTokens { get; set; } = [];
}

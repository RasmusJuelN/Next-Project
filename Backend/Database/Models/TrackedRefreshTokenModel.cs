using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[Table("TrackedRefreshToken")]
[Index(nameof(Token))]
public class TrackedRefreshTokenModel
{
    [Key]
    public int Id { get; set; }

    [Required]    
    public required Guid UserGuid { get; set; }
    
    [Required]
    public required byte[] Token { get; set; }
    
    // Default value configured in Fluent API
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public bool IsRevoked { get; set; }
}

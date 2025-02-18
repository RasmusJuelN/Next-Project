using System.Reflection.Metadata;

namespace Database.Models;

public class RevokedRefreshTokenModel
{
    public int Id { get; set; }
    public required byte[] Token { get; set; }
    public DateTime RevokedAt { get; set; }
}

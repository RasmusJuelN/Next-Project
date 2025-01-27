using System.Reflection.Metadata;

namespace Database.Models;

public class RevokedRefreshTokenModel
{
    public int Id { get; set; }
    public Blob Token { get; set; }
    public DateTime RevokedAt { get; set; }
}

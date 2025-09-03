using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public sealed class RefreshRequest
{
    [Required]
    public string ExpiredToken { get; init; } = default!;
}
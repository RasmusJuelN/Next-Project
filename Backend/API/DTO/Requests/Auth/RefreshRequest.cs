
public sealed class RefreshRequest
{
    [Required]
    public string ExpiredToken { get; init; } = default!;
}

namespace Settings.Default;

public class DefaultSystem : ISystemSettings
{
    public string ListenIP { get; set; } = "localhost";
    public int HttpPort { get; set; } = 5284;
    public int HttpsPort { get; set; } = 7135;
    public bool UseSSL { get; set; } = false;
    public string PfxCertificatePath { get; set; } = string.Empty;
}

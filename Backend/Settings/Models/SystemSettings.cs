
namespace Settings.Models;

public class SystemSettings : Base, ISystemSettings
{
    public override string Key { get; } = "System";

    public string ListenIP { get; set; } = string.Empty;
    public int HttpPort { get; set; }
    public int HttpsPort { get; set; }
    public bool UseSSL { get; set; }
    public string PfxCertificatePath { get; set; } = string.Empty;
}

namespace Settings.Interfaces;

public interface ISystemSettings
{
    public string ListenIP { get; set; }
    public int HttpPort { get; set; }
    public int HttpsPort { get; set; }
    public bool UseSSL { get; set; }
    public string PfxCertificatePath { get; set; }
}

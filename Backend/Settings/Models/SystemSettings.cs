using System.ComponentModel;
using System.Text.Json.Serialization;
using Settings.Interfaces;

namespace Settings.Models;

public class SystemSettings : Base, ISystemSettings
{
    [JsonIgnore]
    public override string Key { get; } = "System";

    [Description("The IP address that the API should listen on for incoming requests.")]
    public string ListenIP { get; set; } = "localhost";

    [Description("The HTTP port number that the API should listen on for incoming requests.")]
    public int HttpPort { get; set; } = 5284;

    [Description("The HTTPS port number that the API should listen on for secure communication.")]
    public int HttpsPort { get; set; } = 7135;

    [Description("Controls if the API should use SSL for secure communication. Not a requirement if behind a reverse proxy.")]
    public bool UseSSL { get; set; } = false;

    [Description("The file path to the PFX certificate used for SSL/TLS encryption if SSL is enabled and a self-signed certificate is used.")]
    public string PfxCertificatePath { get; set; } = string.Empty;
}

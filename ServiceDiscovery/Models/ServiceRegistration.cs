namespace ServiceDiscovery.Models;

public class ServiceRegistration
{
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceUrl { get; set; } = string.Empty;
    public DateTime RegistrationTime { get; set; }
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
}

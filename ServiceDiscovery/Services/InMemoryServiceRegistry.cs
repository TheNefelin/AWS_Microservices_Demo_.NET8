using ServiceDiscovery.Models;

namespace ServiceDiscovery.Services;

public class InMemoryServiceRegistry : IServiceRegistry
{
    private readonly Dictionary<string, List<ServiceRegistration>> _services = new();

    public void RegisterService(string serviceName, string serviceUrl)
    {
        if (!_services.ContainsKey(serviceName))
        {
            _services[serviceName] = new List<ServiceRegistration>();
        }

        // Evitar duplicados
        if (!_services[serviceName].Any(s => s.ServiceUrl == serviceUrl))
        {
            _services[serviceName].Add(new ServiceRegistration
            {
                ServiceName = serviceName,
                ServiceUrl = serviceUrl,
                RegistrationTime = DateTime.UtcNow
            });
        }
    }

    public void UnregisterService(string serviceName, string serviceUrl)
    {
        if (_services.ContainsKey(serviceName))
        {
            _services[serviceName].RemoveAll(s => s.ServiceUrl == serviceUrl);
        }
    }

    public List<string> GetServiceUrls(string serviceName)
    {
        return _services.ContainsKey(serviceName)
            ? _services[serviceName].Select(s => s.ServiceUrl).ToList()
            : new List<string>();
    }
}

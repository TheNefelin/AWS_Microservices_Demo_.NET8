using ServiceDiscovery.Models;

namespace ServiceDiscovery.Services;

public class InMemoryServiceRegistry : IServiceRegistry
{
    private readonly Dictionary<string, List<ServiceRegistration>> _services = new();
    private readonly object _lock = new();

    public void RegisterService(string serviceName, string serviceUrl)
    {
        lock (_lock)
        {
            if (!_services.ContainsKey(serviceName))
                _services[serviceName] = new List<ServiceRegistration>();

            var existing = _services[serviceName].FirstOrDefault(s => s.ServiceUrl == serviceUrl);
            if (existing == null)
            {
                _services[serviceName].Add(new ServiceRegistration
                {
                    ServiceName = serviceName,
                    ServiceUrl = serviceUrl,
                    RegistrationTime = DateTime.UtcNow,
                    LastSeen = DateTime.UtcNow
                });
            }
            else
            {
                existing.LastSeen = DateTime.UtcNow;
            }
        }
    }

    public void UnregisterService(string serviceName, string serviceUrl)
    {
        lock (_lock)
        {
            if (_services.ContainsKey(serviceName))
                _services[serviceName].RemoveAll(s => s.ServiceUrl == serviceUrl);
        }
    }

    public List<string> DiscoverServices(string serviceName)
    {
        lock (_lock)
        {
            if (!_services.ContainsKey(serviceName)) return new List<string>();

            return _services[serviceName]
                .Where(s => s.LastSeen > DateTime.UtcNow.AddSeconds(-90)) // solo vivos
                .Select(s => s.ServiceUrl)
                .ToList();
        }
    }

    public void Heartbeat(string serviceName, string serviceUrl)
    {
        lock (_lock)
        {
            var existing = _services.ContainsKey(serviceName)
                ? _services[serviceName].FirstOrDefault(s => s.ServiceUrl == serviceUrl)
                : null;

            if (existing != null)
                existing.LastSeen = DateTime.UtcNow;
        }
    }

    public void Cleanup()
    {
        lock (_lock)
        {
            foreach (var key in _services.Keys.ToList())
            {
                _services[key].RemoveAll(s => s.LastSeen < DateTime.UtcNow.AddSeconds(-90));
            }
        }
    }
}

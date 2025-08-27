namespace ServiceDiscovery.Services;

public interface IServiceRegistry
{
    void RegisterService(string serviceName, string serviceUrl);
    void UnregisterService(string serviceName, string serviceUrl);
    List<string> DiscoverServices(string serviceName);
    void Heartbeat(string serviceName, string serviceUrl);
    void Cleanup();
}

namespace ServiceDiscovery.Services;

public interface IServiceRegistry
{
    void RegisterService(string serviceName, string serviceUrl);
    void UnregisterService(string serviceName, string serviceUrl);
    List<string> GetServiceUrls(string serviceName);
}

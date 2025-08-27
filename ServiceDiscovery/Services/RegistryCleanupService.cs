namespace ServiceDiscovery.Services;

public class RegistryCleanupService : BackgroundService
{
    private readonly IServiceRegistry _serviceRegistry;

    public RegistryCleanupService(IServiceRegistry serviceRegistry)
    {
        _serviceRegistry = serviceRegistry;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _serviceRegistry.Cleanup();
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}

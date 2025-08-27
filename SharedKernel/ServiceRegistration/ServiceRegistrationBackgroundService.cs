using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace SharedKernel.ServiceRegistration;

public class ServiceRegistrationBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ServiceRegistrationBackgroundService> _logger;
    private readonly string _serviceName;
    private readonly string _serviceUrl;
    private readonly string _discoveryServiceUrl;

    public ServiceRegistrationBackgroundService(
        IHttpClientFactory httpClientFactory,
        ILogger<ServiceRegistrationBackgroundService> logger,
        string serviceName,
        string serviceUrl,
        string discoveryServiceUrl = "http://localhost:5000")
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _serviceName = serviceName;
        _serviceUrl = serviceUrl;
        _discoveryServiceUrl = discoveryServiceUrl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Esperar a que el servicio esté listo
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        using var client = _httpClientFactory.CreateClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var registerUrl = $"{_discoveryServiceUrl}/api/registry/register?serviceName={_serviceName}&serviceUrl={WebUtility.UrlEncode(_serviceUrl)}";
                _logger.LogInformation("Attempting to register service at: {RegisterUrl}", registerUrl);

                var response = await client.PostAsync(registerUrl, null, stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully registered {ServiceName} at {ServiceUrl}", _serviceName, _serviceUrl);
                }
                else
                {
                    _logger.LogWarning("Failed to register service. Status code: {StatusCode}", response.StatusCode);
                }

                // Re-registrar cada 30 segundos para mantener vivo el registro
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register service: {ErrorMessage}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Intentar desregistrar el servicio al detenerse
        try
        {
            using var client = _httpClientFactory.CreateClient();
            var unregisterUrl = $"{_discoveryServiceUrl}/api/registry/unregister?serviceName={_serviceName}&serviceUrl={WebUtility.UrlEncode(_serviceUrl)}";
            await client.PostAsync(unregisterUrl, null, cancellationToken);
            _logger.LogInformation("Successfully unregistered {ServiceName}", _serviceName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unregister service during shutdown");
        }

        await base.StopAsync(cancellationToken);
    }
}
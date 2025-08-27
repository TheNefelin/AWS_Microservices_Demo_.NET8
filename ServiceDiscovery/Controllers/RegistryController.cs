using Microsoft.AspNetCore.Mvc;
using ServiceDiscovery.Services;

namespace ServiceDiscovery.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegistryController : ControllerBase
{
    private readonly IServiceRegistry _serviceRegistry;

    public RegistryController(IServiceRegistry serviceRegistry)
    {
        _serviceRegistry = serviceRegistry;
    }

    [HttpPost("register")]
    public IActionResult RegisterService([FromQuery] string serviceName, [FromQuery] string serviceUrl)
    {
        _serviceRegistry.RegisterService(serviceName, serviceUrl);
        return Ok($"Service {serviceName} registered at {serviceUrl}");
    }

    [HttpPost("unregister")]
    public IActionResult UnregisterService([FromQuery] string serviceName, [FromQuery] string serviceUrl)
    {
        _serviceRegistry.UnregisterService(serviceName, serviceUrl);
        return Ok($"Service {serviceName} unregistered from {serviceUrl}");
    }

    [HttpGet("discover")]
    public IActionResult DiscoverService([FromQuery] string serviceName)
    {
        var urls = _serviceRegistry.GetServiceUrls(serviceName);
        return Ok(urls);
    }
}

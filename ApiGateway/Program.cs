using System.Text.Json;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Agregar servicios necesarios para YARP
builder.Services.AddHttpForwarder(); // Servicio para forward de requests
builder.Services.AddHttpClient();    // HttpClient factory

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configuración de rutas para el reverse proxy
var routes = new[]
{
    new
    {
        RouteId = "products-route",
        ClusterId = "products-cluster",
        Match = new { Path = "/api/products/{**catch-all}" }
    },
    new
    {
        RouteId = "orders-route",
        ClusterId = "orders-cluster",
        Match = new { Path = "/api/orders/{**catch-all}" }
    }
};

// Configuración de clusters para los servicios
var clusters = new[]
{
    new
    {
        ClusterId = "products-cluster",
        Destinations = new Dictionary<string, object>
        {
            ["destination1"] = new { Address = "http://localhost:5001" }
        }
    },
    new
    {
        ClusterId = "orders-cluster",
        Destinations = new Dictionary<string, object>
        {
            ["destination1"] = new { Address = "http://localhost:5002" }
        }
    }
};

// Middleware personalizado para el routing
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
    var forwarder = context.RequestServices.GetRequiredService<IHttpForwarder>();

    try
    {
        // Determinar a qué servicio redirigir basado en la ruta
        string targetService = null;

        if (context.Request.Path.StartsWithSegments("/api/products"))
        {
            logger.LogInformation("Routing request to Product Service");

            // Intentar descubrir el servicio dinámicamente
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync("http://localhost:5000/api/registry/discover?serviceName=ProductService");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var serviceUrls = JsonSerializer.Deserialize<List<string>>(content);

                    if (serviceUrls != null && serviceUrls.Count > 0)
                    {
                        targetService = serviceUrls[0];
                        logger.LogInformation("Discovered Product Service at: {ServiceUrl}", targetService);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Service discovery failed, using fallback");
            }

            // Fallback a la dirección estática
            targetService ??= "http://localhost:5001";
        }
        else if (context.Request.Path.StartsWithSegments("/api/orders"))
        {
            logger.LogInformation("Routing request to Order Service");

            // Intentar descubrir el servicio dinámicamente
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync("http://localhost:5000/api/registry/discover?serviceName=OrderService");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var serviceUrls = JsonSerializer.Deserialize<List<string>>(content);

                    if (serviceUrls != null && serviceUrls.Count > 0)
                    {
                        targetService = serviceUrls[0];
                        logger.LogInformation("Discovered Order Service at: {ServiceUrl}", targetService);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Service discovery failed, using fallback");
            }

            // Fallback a la dirección estática
            targetService ??= "http://localhost:5002";
        }

        // Si encontramos un servicio target, forwardear el request
        if (targetService != null)
        {
            // Crear la URL de destino
            var targetUri = new Uri(new Uri(targetService), context.Request.Path + context.Request.QueryString);

            logger.LogInformation("Forwarding request to: {TargetUri}", targetUri);

            // Forwardear el request
            var error = await forwarder.SendAsync(context, targetUri.ToString(), httpClientFactory.CreateClient());

            // Verificar si hubo error en el forward
            if (error != ForwarderError.None)
            {
                var errorFeature = context.GetForwarderErrorFeature();
                logger.LogError("Forwarding error: {Error}", errorFeature?.Exception?.Message);
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service unavailable");
            }

            return; // Terminar el middleware después de forwardear
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error in reverse proxy middleware");
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Internal server error");
        return;
    }

    // Si no es una ruta que manejamos, continuar
    await next();
});

// Health check endpoint
app.MapGet("/health", () => "API Gateway is running");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

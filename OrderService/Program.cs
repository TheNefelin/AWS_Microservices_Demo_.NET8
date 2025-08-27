using OrderService.Services;
using SharedKernel.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Obtener la URL del servicio
var serviceUrl = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5002";
var discoveryServiceUrl = builder.Configuration["ServiceDiscoveryUrl"] ?? "http://localhost:5000";
builder.Services.AddSingleton<IOrderService, InMemoryOrderService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔥 ESTA LÍNEA ES CRÍTICA - Registrar HttpClientFactory
builder.Services.AddHttpClient();

// Registrar el servicio de fondo con parámetros específicos
builder.Services.AddHostedService(provider =>
    new ServiceRegistrationBackgroundService(
        provider.GetRequiredService<IHttpClientFactory>(),
        provider.GetRequiredService<ILogger<ServiceRegistrationBackgroundService>>(),
        "OrderService",
        serviceUrl,
        discoveryServiceUrl));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("./swagger/v1/swagger.json", "API v1"); // Usa "./" para compatibilidad
    c.RoutePrefix = string.Empty;
    c.DisplayRequestDuration(); // Opcional: muestra tiempo de respuesta
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 🔥 AGREGAR ENDPOINT HEALTH
app.MapGet("/health", () => "OrderService is healthy");

app.MapControllers();

app.Run();

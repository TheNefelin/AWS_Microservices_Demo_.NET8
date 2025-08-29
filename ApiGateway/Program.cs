using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var serviceUrl = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5003";

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔥 OBTENER LAS URLs DESDE CONFIGURACIÓN (appsettings.json o variables de entorno)
var productServiceUrl = builder.Configuration["Services:ProductService:Url"] ?? "http://localhost:5001";
var orderServiceUrl = builder.Configuration["Services:OrderService:Url"] ?? "http://localhost:5002";

Console.WriteLine($"🚀 Product Service URL: {productServiceUrl}");
Console.WriteLine($"🚀 Order Service URL: {orderServiceUrl}");


// Configuración de rutas para el reverse proxy
var routes = new[]
{
    new RouteConfig
    {
        RouteId = "products-route",
        ClusterId = "products-cluster",
        Match = new RouteMatch
        {
            Path = "/api/products/{**catch-all}"
        }
    },
    new RouteConfig
    {
        RouteId = "orders-route",
        ClusterId = "orders-cluster",
        Match = new RouteMatch
        {
            Path = "/api/orders/{**catch-all}"
        }
    }
};

// 🔥 CONFIGURACIÓN DINÁMICA DE CLUSTERS
var clusters = new[]
{
    new ClusterConfig
    {
        ClusterId = "products-cluster",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            {
                "destination1",
                new DestinationConfig
                {
                    Address = productServiceUrl.EndsWith("/") ? productServiceUrl : $"{productServiceUrl}/"
                }
            }
        }
    },
    new ClusterConfig
    {
        ClusterId = "orders-cluster",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            {
                "destination1",
                new DestinationConfig
                {
                    Address = orderServiceUrl.EndsWith("/") ? orderServiceUrl : $"{orderServiceUrl}/"
                }
            }
        }
    }
};

// Configuración de YARP
builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters);

var app = builder.Build();

// Health check endpoint
app.MapGet("/health", () => "API Gateway is running");

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
app.MapControllers();

// ✅ Esto es lo más importante - Mapear el reverse proxy
app.MapReverseProxy();

app.Run();

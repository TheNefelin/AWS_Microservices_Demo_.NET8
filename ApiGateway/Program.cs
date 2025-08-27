using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configuración de clusters para los servicios
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
                    Address = "http://localhost:5001/"
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
                    Address = "http://localhost:5002/"
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

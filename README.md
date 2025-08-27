# Aplicación Simple con Patrones de Microservicios en .NET 8
- Una aplicación sencilla que implemente varios patrones comunes de microservicios. Crearemos una solución con API Gateway, servicio de descubrimiento (similar a Eureka), y aplicaremos principios como Open/Closed.

### Estructura de la solución
```
MicroservicesDemo/
├── ApiGateway/                 # API Gateway (YARP)
├── ServiceDiscovery/           # Servicio de descubrimiento
├── ProductService/             # Microservicio de productos
├── OrderService/               # Microservicio de órdenes
└── SharedModels/               # Modelos compartidos
```

### Dependencias entre proyectos
- SharedModels (Class Library)
- ServiceDiscovery (ASP.NET Core Web API)
- ProductService (ASP.NET Core Web API)
```
SharedModels
```
- OrderService (ASP.NET Core Web API)
```
SharedModels
```
- API Gateway (ASP.NET Core Web API)
```
Yarp.ReverseProxy
```


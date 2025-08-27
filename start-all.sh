#!/bin/bash
# start-all.sh

# Iniciar Service Discovery
echo "Starting Service Discovery..."
dotnet run --project ServiceDiscovery --urls="http://localhost:5000" &

# Esperar un poco para que el service discovery inicie
sleep 5

# Iniciar Product Service
echo "Starting Product Service..."
dotnet run --project ProductService --urls="http://localhost:5001" &

# Iniciar Order Service
echo "Starting Order Service..."
dotnet run --project OrderService --urls="http://localhost:5002" &

# Esperar a que los servicios inicien
sleep 5

# Iniciar API Gateway
echo "Starting API Gateway..."
dotnet run --project ApiGateway --urls="http://localhost:5003" &

echo "All services are starting..."
echo "Service Discovery: http://localhost:5000"
echo "Product Service: http://localhost:5001"
echo "Order Service: http://localhost:5002"
echo "API Gateway: http://localhost:5003"

# Esperar a que todos los servicios terminen (Ctrl+C para detener)
wait

@echo off
echo Starting Microservices...

echo Starting Service Discovery on port 5000...
start "ServiceDiscovery" dotnet run --project ServiceDiscovery\ServiceDiscovery.csproj --urls http://localhost:5000
timeout /t 2 /nobreak >nul

echo Starting Product Service on port 5001...
start "ProductService" dotnet run --project ProductService\ProductService.csproj --urls http://localhost:5001
timeout /t 2 /nobreak >nul

echo Starting Order Service on port 5002...
start "OrderService" dotnet run --project OrderService\OrderService.csproj --urls http://localhost:5002
timeout /t 2 /nobreak >nul

echo Waiting for services to start...
timeout /t 5 /nobreak >nul

echo Starting API Gateway on port 5003...
start "ApiGateway" dotnet run --project ApiGateway\ApiGateway.csproj --urls http://localhost:5003

echo All services are starting...
echo Service Discovery: http://localhost:5000
echo Product Service: http://localhost:5001
echo Order Service: http://localhost:5002
echo API Gateway: http://localhost:5003
echo.
echo Press any key to stop all services...
pause >nul

echo Stopping all services...
taskkill /f /im dotnet.exe >nul 2>&1
echo All services stopped.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/WebApiService/WebApiService.csproj", "Services/WebApiService/"]
COPY ["BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBus/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBus/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/EventBus/"]
COPY ["BuildingBlocks/FibonacciCalculator/FibonacciCalculator.csproj", "BuildingBlocks/FibonacciCalculator/"]
RUN dotnet restore "Services/WebApiService/WebApiService.csproj"
COPY . .
WORKDIR "/src/Services/WebApiService"
RUN dotnet build "WebApiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiService.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/RabbitMQProducer/RabbitMQProducer.csproj", "Services/RabbitMQProducer/"]
COPY ["BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBus/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBus/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/EventBus/"]
COPY ["BuildingBlocks/FibonacciCalculator/FibonacciCalculator.csproj", "BuildingBlocks/FibonacciCalculator/"]
RUN dotnet restore "Services/RabbitMQProducer/RabbitMQProducer.csproj"
COPY . .
WORKDIR "/src/Services/RabbitMQProducer"
RUN dotnet build "RabbitMQProducer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMQProducer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMQProducer.dll"]
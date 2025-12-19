FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/PricingService/PricingService.csproj", "Services/PricingService/"]
COPY ["src/Shared/Shared.Observability/Shared.Observability.csproj", "Shared/Shared.Observability/"]
COPY ["src/Shared/Shared.Kafka/Shared.Kafka.csproj", "Shared/Shared.Kafka/"]

RUN dotnet restore "Services/PricingService/PricingService.csproj"

COPY src/ .

WORKDIR "/src/Services/PricingService"
RUN dotnet build "PricingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PricingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PricingService.dll"]

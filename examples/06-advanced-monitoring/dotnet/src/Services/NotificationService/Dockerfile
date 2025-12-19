FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/NotificationService/NotificationService.csproj", "Services/NotificationService/"]
COPY ["src/Shared/Shared.Observability/Shared.Observability.csproj", "Shared/Shared.Observability/"]
COPY ["src/Shared/Shared.Kafka/Shared.Kafka.csproj", "Shared/Shared.Kafka/"]

RUN dotnet restore "Services/NotificationService/NotificationService.csproj"

COPY src/ .

WORKDIR "/src/Services/NotificationService"
RUN dotnet build "NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.dll"]

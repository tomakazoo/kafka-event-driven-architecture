FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/NavCalculator/NavCalculator.csproj", "Services/NavCalculator/"]
COPY ["src/Shared/Shared.Observability/Shared.Observability.csproj", "Shared/Shared.Observability/"]
COPY ["src/Shared/Shared.Kafka/Shared.Kafka.csproj", "Shared/Shared.Kafka/"]

RUN dotnet restore "Services/NavCalculator/NavCalculator.csproj"

COPY src/ .

WORKDIR "/src/Services/NavCalculator"
RUN dotnet build "NavCalculator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NavCalculator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NavCalculator.dll"]

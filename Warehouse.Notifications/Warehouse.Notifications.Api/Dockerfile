FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Warehouse.Notifications/Warehouse.Notifications.Domain/Warehouse.Notifications.Domain.csproj", "Warehouse.Notifications/Warehouse.Notifications.Domain/"]
COPY ["Warehouse.Notifications/Warehouse.Notifications.Application/Warehouse.Notifications.Application.csproj", "Warehouse.Notifications/Warehouse.Notifications.Application/"]
COPY ["Warehouse.Notifications/Warehouse.Notifications.Infrastructure/Warehouse.Notifications.Infrastructure.csproj", "Warehouse.Notifications/Warehouse.Notifications.Infrastructure/"]
COPY ["Warehouse.Notifications/Warehouse.Notifications.Api/Warehouse.Notifications.Api.csproj", "Warehouse.Notifications/Warehouse.Notifications.Api/"]

RUN dotnet restore "Warehouse.Notifications/Warehouse.Notifications.Api/Warehouse.Notifications.Api.csproj"

COPY . .

WORKDIR "/src/Warehouse.Notifications/Warehouse.Notifications.Api"
RUN dotnet publish "Warehouse.Notifications.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Warehouse.Notifications.Api.dll"]

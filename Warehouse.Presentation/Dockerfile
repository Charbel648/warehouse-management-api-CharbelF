FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Warehouse.Domain/Warehouse.Domain.csproj", "Warehouse.Domain/"]
COPY ["Warehouse.Application/Warehouse.Application.csproj", "Warehouse.Application/"]
COPY ["Warehouse.Infrastructure/Warehouse.Infrastructure.csproj", "Warehouse.Infrastructure/"]
COPY ["Warehouse.Presentation/Warehouse.Presentation.csproj", "Warehouse.Presentation/"]

RUN dotnet restore "Warehouse.Presentation/Warehouse.Presentation.csproj"

COPY . .

WORKDIR "/src/Warehouse.Presentation"
RUN dotnet publish "Warehouse.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Warehouse.Presentation.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Fabiq.SmartFactory.Simulator.csproj", "./"]
RUN dotnet restore "Fabiq.SmartFactory.Simulator.csproj"

COPY . .
RUN dotnet publish "Fabiq.SmartFactory.Simulator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fabiq.SmartFactory.Simulator.dll"]
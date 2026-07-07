FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Fabiq.SmartFactory.Api.csproj", "./"]
RUN dotnet restore "Fabiq.SmartFactory.Api.csproj"

COPY . .
RUN dotnet publish "Fabiq.SmartFactory.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:5078
EXPOSE 5078

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fabiq.SmartFactory.Api.dll"]
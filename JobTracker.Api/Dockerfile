# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["JobTracker.Api.csproj", "./"]
RUN dotnet restore "JobTracker.Api.csproj"

# Copy everything else and build
COPY . .
RUN dotnet publish "JobTracker.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (Render sets PORT env var, ASP.NET Core needs to listen on it)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "JobTracker.Api.dll"]
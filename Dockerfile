FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for better Docker layer caching
COPY server/Directory.Packages.props server/global.json server/Manager.sln ./server/
COPY server/ManagerGame/ManagerGame.csproj ./server/ManagerGame/
COPY server/ManagerGame.Core/ManagerGame.Core.csproj ./server/ManagerGame.Core/
COPY server/ManagerGame.Domain/ManagerGame.Domain.csproj ./server/ManagerGame.Domain/

RUN dotnet restore ./server/ManagerGame/ManagerGame.csproj

# Copy the rest of the source and publish
COPY server ./server
RUN dotnet publish ./server/ManagerGame/ManagerGame.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app
USER appuser

EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ManagerGame.dll"]



# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for restore caching
COPY MusicPlaylistAPI.sln ./
COPY MusicPlaylistAPI/MusicPlaylistAPI.csproj MusicPlaylistAPI/
RUN dotnet restore MusicPlaylistAPI/MusicPlaylistAPI.csproj

# Copy the rest of the source
COPY MusicPlaylistAPI/ MusicPlaylistAPI/

# Publish for runtime
RUN dotnet publish MusicPlaylistAPI/MusicPlaylistAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Environment: bind to 0.0.0.0:8080 by default
ENV ASPNETCORE_URLS=http://0.0.0.0:5099
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_ENVIRONMENT=Production

# Optional: configure timezone or globalization if needed
# ENV TZ=Europe/Kiev

# Copy published output
COPY --from=build /app/publish .

# Expose the port your app listens on
EXPOSE 5099

# Health check (optional)
# HEALTHCHECK --interval=30s --timeout=3s CMD wget -qO- http://localhost:8080/health || exit 1

# Run the app
ENTRYPOINT ["dotnet", "MusicPlaylistAPI.dll"]
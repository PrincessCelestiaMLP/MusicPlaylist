# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копіюємо csproj окремо для кешування restore
COPY ./MusicPlaylistAPI/MusicPlaylistAPI.csproj ./MusicPlaylistAPI/
RUN dotnet restore MusicPlaylistAPI/MusicPlaylistAPI.csproj

# Копіюємо весь код
COPY MusicPlaylistAPI/* MusicPlaylistAPI/

# Публікуємо
RUN dotnet publish MusicPlaylistAPI/MusicPlaylistAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Змінні середовища
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Копіюємо з білд-стейджу
COPY --from=build /app/publish .

# Відкриваємо порт
EXPOSE 8080

# Запускаємо
ENTRYPOINT ["dotnet", "MusicPlaylistAPI.dll"]
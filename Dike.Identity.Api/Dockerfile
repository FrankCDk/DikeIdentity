# Fase 1: Base de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
# Exponemos ambos puertos
EXPOSE 8080
EXPOSE 8081

# Configuración HTTPS Local
ENV ASPNETCORE_URLS="https://+:8081;http://+:8080"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.dotnet/https/dike_dev_cert.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="Frank1234"

# Fase 2: Compilación de la solución completa
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamos todos los archivos de definición de proyectos (.csproj) respetando las carpetas hermanos
COPY ["Dike.Identity.Api/Dike.Identity.Api.csproj", "Dike.Identity.Api/"]
COPY ["Dike.Identity.Core/Dike.Identity.Core.csproj", "Dike.Identity.Core/"]
COPY ["Dike.Identity.Providers.Cache/Dike.Identity.Providers.Cache.csproj", "Dike.Identity.Providers.Cache/"]
COPY ["Dike.Identity.Providers.Jwt/Dike.Identity.Providers.Jwt.csproj", "Dike.Identity.Providers.Jwt/"]
COPY ["DikeIdentity.Providers.Persistence/Dike.Identity.Providers.Persistence.csproj", "Dike.Identity.Providers.Persistence/"]
COPY ["Dike.Identity.Providers.Security/Dike.Identity.Providers.Security.csproj", "Dike.Identity.Providers.Security/"]

# Restauramos las dependencias desde el proyecto ejecutable principal
RUN dotnet restore "Dike.Identity.Api/Dike.Identity.Api.csproj"

# Copiamos el resto del código fuente de todas las capas
COPY . .
WORKDIR "/src/Dike.Identity.Api"
RUN dotnet build "Dike.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Fase 3: Publicación de artefactos optimizados
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Dike.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase 4: Imagen Final armada
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dike.Identity.Api.dll"]
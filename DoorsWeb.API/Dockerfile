# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Pre-create the mounted data directories and hand them to the non-root runtime
# user ($APP_UID). When Docker first mounts an *empty* named volume over one of
# these paths it copies this ownership onto the volume, so the app can write to
# /data/settings, /data/backups and /data/user-photo. Without this, fresh named
# volumes are created root-owned and writes fail with "Access to the path ... is
# denied". (Existing root-owned volumes must be recreated or chowned once.)
USER root
RUN mkdir -p /data/backups /data/user-photo /data/settings \
    && chown -R $APP_UID:$APP_UID /data
USER $APP_UID


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DoorsWeb.API/DoorsWeb.API.csproj", "DoorsWeb.API/"]

RUN dotnet restore "./DoorsWeb.API/DoorsWeb.API.csproj"
COPY . .
WORKDIR "/src/DoorsWeb.API"
RUN dotnet build "./DoorsWeb.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DoorsWeb.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoorsWeb.API.dll"]
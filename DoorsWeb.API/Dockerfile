# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# As root we (1) install the PostgreSQL client tools (pg_dump/pg_restore) that
# BackupService shells out to, pinned to major 16 from the official PGDG apt repo to
# match the postgres:16 server, plus libfontconfig1 (the native font library QuestPDF/SkiaSharp
# needs to render report PDFs on Linux), and (2) pre-create the mounted data directories and
# hand them to the non-root runtime user ($APP_UID). When Docker first mounts an
# *empty* named volume over one of these paths it copies this ownership onto the
# volume, so the app can write to /data/settings, /data/backups, /data/user-photo and
# /data/card-photo.
# Without this, fresh named volumes are created root-owned and writes fail with
# "Access to the path ... is denied". (Existing root-owned volumes must be recreated
# or chowned once.)
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates gnupg \
    && install -d /usr/share/postgresql-common/pgdg \
    && curl -fsSL https://www.postgresql.org/media/keys/ACCC4CF8.asc \
         -o /usr/share/postgresql-common/pgdg/apt.postgresql.org.asc \
    && echo "deb [signed-by=/usr/share/postgresql-common/pgdg/apt.postgresql.org.asc] https://apt.postgresql.org/pub/repos/apt $(. /etc/os-release && echo $VERSION_CODENAME)-pgdg main" \
         > /etc/apt/sources.list.d/pgdg.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends postgresql-client-16 libfontconfig1 \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /data/backups /data/user-photo /data/card-photo /data/settings \
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
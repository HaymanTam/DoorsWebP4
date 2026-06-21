# This stage is used to publish the service project to be copied to the final stage

FROM nginx:alpine AS base

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DoorsWeb.Client/DoorsWeb.Client.csproj", "DoorsWeb.Client/"]
RUN dotnet restore "./DoorsWeb.Client/DoorsWeb.Client.csproj"
COPY . .
WORKDIR "/src/DoorsWeb.Client"
RUN dotnet build "./DoorsWeb.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build



# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DoorsWeb.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /usr/share/nginx/html
# WORKDIR /var/www/web
COPY --from=publish /app/publish/wwwroot .
COPY --from=publish "/src/DoorsWeb.Client/nginx.conf" /etc/nginx/nginx.conf

# The TLS certificate is no longer generated at build time. It is mounted at runtime into
# /etc/nginx/certs (see docker-compose.override.yml) from an mkcert-issued, locally-trusted
# cert on the host. For deployment, a per-site setup step drops the site's cert there.

CMD ["nginx", "-g", "daemon off;"]
EXPOSE 443
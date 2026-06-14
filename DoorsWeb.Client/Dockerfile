# This stage is used to publish the service project to be copied to the final stage

FROM nginx:alpine as base

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
FROM base as final
WORKDIR /usr/share/nginx/html
# WORKDIR /var/www/web
COPY --from=publish /app/publish/wwwroot .
COPY --from=publish "/src/DoorsWeb.Client/nginx.conf" /etc/nginx/nginx.conf

# Install openssl
RUN apk add --no-cache openssl
RUN mkdir /etc/nginx/certs

# Generate self-signed certificate
# -x509: Create a self-signed certificate
# -nodes: Don't encrypt the private key
# -subj: Provide certificate info (Country, State, Locality, Org, Common Name)
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /etc/nginx/certs/selfsigned.key \
    -out /etc/nginx/certs/selfsigned.crt \
    -subj "/C=UK/S=Lancashire/L=Accrington/O=BSBProgeny/CN=localhost"

CMD ["nginx", "-g", "daemon off;"]
EXPOSE 443
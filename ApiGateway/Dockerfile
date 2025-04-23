# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=8.0

# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS builder
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY --link ApiGateway.csproj ./
COPY --link ../JwtAuthenticationManager/JwtAuthenticationManager.csproj ../JwtAuthenticationManager/
COPY --link ../SharedLibrary/SharedLibrary.csproj ../SharedLibrary/
COPY --link ../api-microservices-dotnet.ServiceDefaults/api-microservices-dotnet.ServiceDefaults.csproj ../api-microservices-dotnet.ServiceDefaults/

# Restore dependencies with cache mounts
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet restore ApiGateway.csproj

# Copy the rest of the source code
COPY --link . .
COPY --link ../JwtAuthenticationManager ../JwtAuthenticationManager
COPY --link ../SharedLibrary ../SharedLibrary
COPY --link ../api-microservices-dotnet.ServiceDefaults ../api-microservices-dotnet.ServiceDefaults

# Publish the application
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet publish ApiGateway.csproj -c Release -o /app/publish --no-restore

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# Create a non-root user
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

# Copy published output
COPY --from=builder /app/publish .

# Expose the default port for ASP.NET (Ocelot typically uses 80 or 8080)
EXPOSE 8080

ENTRYPOINT ["dotnet", "ApiGateway.dll"]

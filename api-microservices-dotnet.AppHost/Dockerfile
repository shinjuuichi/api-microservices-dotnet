# syntax=docker/dockerfile:1

# Set .NET version from csproj (net8.0)
ARG DOTNET_VERSION=8.0

# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS builder
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY --link api-microservices-dotnet.AppHost.csproj ./
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet restore ./api-microservices-dotnet.AppHost.csproj

# Copy everything else and build
COPY --link . .
RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet publish ./api-microservices-dotnet.AppHost.csproj -c Release -o /app/publish --no-restore

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# Create non-root user
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=builder /app/publish .

# Expose default ports (optional, adjust as needed)
# EXPOSE 17000 15251

ENTRYPOINT ["dotnet", "api-microservices-dotnet.AppHost.dll"]

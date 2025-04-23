# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=8.0

# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS builder
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY --link OrderService/OrderService.csproj ./
COPY --link OrderService/*.csproj ./
# Copy referenced projects' csproj files for restore
COPY --link ../api-microservices-dotnet.ServiceDefaults/api-microservices-dotnet.ServiceDefaults.csproj ../api-microservices-dotnet.ServiceDefaults/
COPY --link ../JwtAuthenticationManager/JwtAuthenticationManager.csproj ../JwtAuthenticationManager/
COPY --link ../RabbitMQ.Contracts/RabbitMQ.Contracts.csproj ../RabbitMQ.Contracts/
COPY --link ../SharedLibrary/SharedLibrary.csproj ../SharedLibrary/

RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet restore "OrderService.csproj"

# Copy everything else and build
COPY --link . .

RUN --mount=type=cache,target=/root/.nuget/packages \
    --mount=type=cache,target=/root/.cache/msbuild \
    dotnet publish "OrderService.csproj" -c Release -o /app/publish --no-restore

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# Security: create non-root user
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=builder /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "OrderService.dll"]

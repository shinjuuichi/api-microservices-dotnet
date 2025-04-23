---

## 🐳 Running with Docker

This project provides full Docker support for local development and deployment. All core services, dependencies, and databases can be started with a single command using **Docker Compose**.

### 🛠️ Requirements
- **Docker** (latest recommended)
- **Docker Compose** (v2+)
- No additional local installations of .NET or SQL Server required

### ⚙️ Service Ports
| Service                | Container Name         | Exposed Port |
|------------------------|-----------------------|--------------|
| **API Gateway**        | csharp-apigateway     | 8080         |
| **AuthService**        | csharp-authservice    | 8081         |
| **ProductService**     | csharp-productservice | 8082         |
| **OrderService**       | csharp-orderservice   | 8083         |
| **RabbitMQ**           | rabbitmq              | 5672, 15672  |
| **SQL Server**         | sqlserver             | 1433         |

> **Note:** All .NET services listen on port 8080 inside their containers, but are mapped to different host ports for convenience.

### 🔑 Environment Variables
- **SQL Server**: The default `SA_PASSWORD` is set to `Your_strong_password123!` in `docker-compose.yml`. **Change this for production use.**
- Each service can be configured further via environment variables or `.env` files (see commented `env_file` lines in the compose file).

### 🚀 Quick Start

1️⃣ **Build and Start All Services**

```sh
docker compose up --build
```

This will build all microservices and supporting containers, then start them together on a shared Docker network.

2️⃣ **Access Services**
- **API Gateway:** http://localhost:8080
- **AuthService:** http://localhost:8081
- **ProductService:** http://localhost:8082
- **OrderService:** http://localhost:8083
- **RabbitMQ UI:** http://localhost:15672 (default user/pass: guest/guest)
- **SQL Server:** localhost:1433 (user: sa, password: as above)

3️⃣ **Database Initialization**
- Databases are created automatically by the services on startup if they do not exist.
- If you need to apply migrations manually, you can still use the `dotnet ef database update` commands inside the respective containers.

### ⚡ Special Notes
- All .NET services are built with **.NET 8** (see `ARG DOTNET_VERSION=8.0` in Dockerfiles).
- Each service runs as a non-root user for improved security.
- The `backend` Docker network and named volumes for RabbitMQ and SQL Server are managed automatically by Docker Compose.
- For custom configuration, you can provide `.env` files in each service directory and uncomment the corresponding `env_file` lines in `docker-compose.yml`.

---

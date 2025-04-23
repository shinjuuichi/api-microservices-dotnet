# ✨ .NET 8 Microservices Architecture

A comprehensive **microservices-based e-commerce solution** built with **.NET 8**, featuring the **API Gateway** pattern, **message-based communication**, and **service isolation** for scalability and maintainability.

---

## 🌐 Project Structure

This solution consists of multiple microservices, each with a specific responsibility:

- **🛡️ ApiGateway** - Routes client requests to appropriate services using **Ocelot**.
- **🔒 AuthService** - Handles **authentication & authorization**.
- **💼 ProductService** - Manages **products & inventory**.
- **🏢 OrderService** - Processes **customer orders**.
- **📝 SharedLibrary** - Common **components & utilities** shared across services.
- **⚖️ JwtAuthenticationManager** - Centralized **authentication logic**.
- **🌏 RabbitMQ.Contracts** - Defines **message contracts** for service communication.

---

## 💻 Technologies Used

| Technology               | Purpose                                    |
|--------------------------|--------------------------------------------|
| **.NET 8**               | Core framework for microservices          |
| **Entity Framework Core**| ORM for database interactions             |
| **SQL Server**           | Relational database for storage           |
| **Ocelot**               | API Gateway for request routing           |
| **MassTransit & RabbitMQ** | Message broker for async communication   |
| **JWT Authentication**   | Secure authentication mechanism           |

---

## ✅ Getting Started

### 🛠️ Prerequisites

Ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/)
- [Docker](https://www.docker.com/) (for RabbitMQ)

### ♻️ Setup Instructions

#### 1️⃣ Clone the Repository
```sh
git clone https://github.com/shinjuuichi/api-microservices-dotnet
```

#### 2️⃣ Set Up Databases
Create the following databases in **SQL Server**:
- `UserDb`
- `ProductDb`
- `OrderDb`

#### 3️⃣ Apply Migrations
```sh
# Authentication
cd JwtAuthenticationManager
dotnet ef database update

# Product Service
cd ProductService
dotnet ef database update

# Order Service
cd OrderService
dotnet ef database update
```

#### 4️⃣ Start RabbitMQ
```sh
docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

#### 5️⃣ Run the Services
Each service should be started in a separate terminal:
```sh
# API Gateway
cd ApiGateway
dotnet run

# Auth Service
cd AuthService
dotnet run

# Product Service
cd ProductService
dotnet run

# Order Service
cd OrderService
dotnet run
```

---

## 📖 API Documentation

### 🔒 Auth Service
- **POST** `/api/v1/auth/register` - Register a new user
- **POST** `/api/v1/auth/login` - Login and obtain JWT token
- **GET** `/api/v1/auth/me` - Get current user info

### 💼 Product Service
- **GET** `/api/v1/product` - Retrieve all products
- **GET** `/api/v1/product/{id}` - Retrieve product by ID
- **POST** `/api/v1/product` - Create a new product
- **PUT** `/api/v1/product/{id}` - Update a product
- **DELETE** `/api/v1/product/{id}` - Delete a product

### 🏢 Order Service
#### 💼 Admin Endpoints
- **GET** `/api/v1/admin/orders` - Retrieve all orders
- **GET** `/api/v1/admin/orders/{id}` - Retrieve order by ID
- **POST** `/api/v1/admin/orders` - Create a new order
- **PUT** `/api/v1/admin/orders/{id}` - Update an order
- **DELETE** `/api/v1/admin/orders/{id}` - Delete an order

#### 👨‍👩‍👦 User Endpoints
- **GET** `/api/v1/user/orders` - Retrieve user orders
- **POST** `/api/v1/user/orders` - Place a new order

## 🛠️ Key Features

### 🌐 API Gateway (Ocelot)
Efficient request routing using **Ocelot**:
```csharp
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
```

### ⚡ Centralized Exception Handling
Unified **exception handling** across all services:
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

### 📡 Service Communication via RabbitMQ
Microservices communicate asynchronously via **MassTransit & RabbitMQ**:
```csharp
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        // Process order and update product inventory
    }
}
```

### 🔐 Secure Authentication (JWT)
All services use **JWT Authentication** for security:
```csharp
builder.Services.AddCustomJwtAuthentication();
```

---

## 🌍 Architecture Overview

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │
┌──────▼──────┐
│ API Gateway │
└──────┬──────┘
       │
┌──────┼──────┬─────────┬─────────┐
│      │      │         │         │
▼      ▼      ▼         ▼         ▼
┌──────────┐ ┌─────────┐ ┌─────────┐
│   Auth   │ │ Product │ │  Order  │
│ Service  │ │ Service │ │ Service │
└──────────┘ └─────────┘ └─────────┘
       ▲           ▲           ▲
       │           │           │
       └───────────┼───────────┘
                   │
             ┌─────▼─────┐
             │ RabbitMQ  │
             └───────────┘
```

---

## 🛡️ Middleware Components

### ⛔ RestrictAccessMiddleware
Controls **service access** based on HTTP **referrer headers**:
```csharp
public class RestrictAccessMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var referrer = context.Request.Headers["Referer"].FirstOrDefault();
        if (string.IsNullOrEmpty(referrer))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access Denied.");
            return;
        }
        await next(context);
    }
}
```

---

## ⚖️ License

This project is licensed under the [MIT License](LICENSE).

---

👍 **Happy Coding!**

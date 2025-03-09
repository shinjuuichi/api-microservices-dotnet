```markdown
# .NET 8 Microservices Architecture

A comprehensive microservices-based e-commerce solution built with .NET 8, featuring API Gateway pattern, message-based communication, and service isolation.

## Project Structure

This solution consists of the following microservices:

- **ApiGateway**: Routes client requests to appropriate services using Ocelot
- **AuthService**: Handles user authentication and authorization
- **ProductService**: Manages product catalog and inventory
- **OrderService**: Processes customer orders
- **SharedLibrary**: Contains common components used across services
- **JwtAuthenticationManager**: Centralized authentication logic
- **RabbitMQ.Contracts**: Message contracts for service communication

## Technologies

- **.NET 8** - Latest version of the .NET platform
- **Entity Framework Core** - ORM for data access
- **SQL Server** - Database for persistent storage
- **Ocelot** - API Gateway implementation
- **MassTransit & RabbitMQ** - Message broker for asynchronous communication
- **JWT Authentication** - Secure identity management

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/)
- [Docker](https://www.docker.com/) (for running RabbitMQ)

### Setting Up Development Environment

1. **Clone the repository**


```
git clone <repository-url>
cd <repository-name>

```

2. **Set up databases**

Create the following databases in SQL Server:
- UserDb
- ProductDb
- OrderDb

3. **Apply database migrations**


```
# For JWT Authentication
dotnet ef database update --project JwtAuthenticationManager

# For Product Service
dotnet ef database update --project ProductService

# For Order Service
dotnet ef database update --project OrderService

```

4. **Start RabbitMQ using Docker**


```
docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management

```

5. **Run the services**

Run each service in a separate terminal:


```
# Gateway
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

## API Documentation

### Auth Service

- **POST /api/v1/auth/register** - Register a new user
- **POST /api/v1/auth/login** - Login and obtain JWT token
- **GET /api/v1/auth/me** - Get current user information

### Product Service

- **GET /api/v1/product** - Get all products
- **GET /api/v1/product/{id}** - Get product by ID
- **POST /api/v1/product** - Create a new product
- **PUT /api/v1/product/{id}** - Update a product
- **DELETE /api/v1/product/{id}** - Delete a product

- **GET /api/v1/category** - Get all categories
- **GET /api/v1/category/{id}** - Get category by ID
- **POST /api/v1/category** - Create a new category
- **PUT /api/v1/category/{id}** - Update a category
- **DELETE /api/v1/category/{id}** - Delete a category

### Order Service

- **GET /api/v1/admin/orders** - Get all orders (admin)
- **GET /api/v1/admin/orders/{id}** - Get order by ID (admin)
- **POST /api/v1/admin/orders** - Create new order (admin)
- **PUT /api/v1/admin/orders/{id}** - Update order (admin)
- **DELETE /api/v1/admin/orders/{id}** - Delete order (admin)

## Key Features

### API Gateway

The API Gateway uses Ocelot to route client requests to the appropriate microservice.


```
// From ApiGateway/Program.cs
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

```

### Centralized Exception Handling

All services implement a unified exception handling approach:


```
// From SharedLibrary/Middlewares/ExceptionHandlingMiddleware.cs
app.UseMiddleware<ExceptionHandlingMiddleware>();

```

### Service Communication via RabbitMQ

Services communicate asynchronously using MassTransit and RabbitMQ:


```
// Example consumer in ProductService
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        // Process order and update product inventory
    }
}

```

### JWT Authentication

Secure authentication across all services using JWT:


```
builder.Services.AddCustomJwtAuthentication();

```

## Architecture


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

## Middleware Components

### RestrictAccessMiddleware

Controls access to services based on HTTP referrer headers:


```
public class RestrictAccessMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var referrer = context.Request.Headers["Referer"].FirstOrDefault();
        if (string.IsNullOrEmpty(referrer))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Cant reach.");
            return;
        }
        else
        {
            await next(context);
        }
    }
}

```

## License

[MIT License](LICENSE)

```

This README.md provides a comprehensive overview of your .NET 8 microservices application, including how to set it up, the architecture, available endpoints, and key features. You can modify any sections as needed to better match your project's specific details and requirements.

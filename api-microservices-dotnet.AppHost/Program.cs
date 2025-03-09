using EnvDTE;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ApiGateway>("apigateway");

builder.AddProject<Projects.AuthService>("authservice");

builder.AddProject<Projects.OrderService>("orderservice");

builder.AddProject<Projects.ProductService>("productservice");

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

//var sqlServer = builder.AddSqlServer("sql");
//var userDb = sqlServer.AddDatabase("UserDb");
//var orderDb = sqlServer.AddDatabase("OrderDb");
//var productDb = sqlServer.AddDatabase("ProductDb");

//var rabbitMq = builder.AddRabbitMQ("rabbitmq")
//    .WithManagementPlugin();

builder.AddProject<Projects.ApiGateway>("apigateway");

builder.AddProject<Projects.AuthService>("authservice");
//.WithReference(userDb)
//.WithReference(rabbitMq);

builder.AddProject<Projects.OrderService>("orderservice");
//.WithReference(orderDb)
//.WithReference(rabbitMq);

builder.AddProject<Projects.ProductService>("productservice");
//.WithReference(productDb)
//.WithReference(rabbitMq);

builder.Build().Run();
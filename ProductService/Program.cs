using JwtAuthenticationManager;
using ProductService;
using ProductService.Messaging.Events;
using ProductService.Messaging.Handlers;
using ProductService.Messaging.RabbitMQ;
using SharedLibrary.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddInfrastructureService();
builder.Services.AddWebAPIService();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddRabbitListeners();

builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRabbitListeners(new List<Type>
{
    typeof(OrderCreatedEvent),
    typeof(OrderStockValidatedEvent),
});

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RestrictAccessMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

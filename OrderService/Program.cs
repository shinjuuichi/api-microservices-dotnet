using JwtAuthenticationManager;
using OrderService;
using OrderService.Messaging.Events;
using OrderService.Messaging.Handlers;
using OrderService.Messaging.RabbitMQ;
using SharedLibrary.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructureService();
builder.Services.AddWebAPIService();
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddLogging();
builder.Services.AddRabbitListeners();

builder.Services.AddScoped<IEventHandler<ProductUpdatedEvent>, ProductUpdatedEventHandler>();
builder.Services.AddScoped<IEventHandler<UserUpdatedEvent>, UserUpdatedEventHandler>();
builder.Services.AddScoped<IEventHandler<OrderStockValidatedEvent>, OrderStockValidatedEventHandler>();



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
    typeof(ProductUpdatedEvent),
    typeof(UserUpdatedEvent)
});


app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RestrictAccessMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

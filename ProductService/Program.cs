using JwtAuthenticationManager;
using ProductService.DependencyInjection.Extensions;
using SharedLibrary.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddInfrastructureService();
builder.Services.AddWebAPIService();

// Add MassTransit RabbitMQ
builder.Services.AddRabbitMQServices(builder.Configuration);

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RestrictAccessMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

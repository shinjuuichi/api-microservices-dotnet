using JwtAuthenticationManager;
using OrderService.DependencyInjection.Extensions;
using OrderService.DependencyInjection.Extensions.DependencyInjection.Extensions;
using SharedLibrary.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructureService();
builder.Services.AddWebAPIService();
builder.Services.AddCustomJwtAuthentication();
builder.Services.AddRabbitMQServices(builder.Configuration);

builder.Services.AddScoped<ExceptionHandlingMiddleware>();


var app = builder.Build();

// Configure the HTTP request pipeline.
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

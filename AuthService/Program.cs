using AuthService.DependencyInjection.Extensions;
using JwtAuthenticationManager;
using SharedLibrary.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddWebAPIService();
builder.Services.AddCustomJwtAuthentication();

// Add MassTransit RabbitMQ
builder.Services.AddRabbitMQServices(builder.Configuration);

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RestrictAccessMiddleware>();

app.MapControllers();

app.Run();

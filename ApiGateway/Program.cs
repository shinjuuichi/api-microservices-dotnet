using ApiGateway.Middlewares;
using Gateway.Middlewares;
using JwtAuthenticationManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SharedLibrary.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

var apiPolicy = "MicroservicesPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(apiPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:3000")
       .AllowAnyMethod()
       .AllowAnyHeader()
       .AllowCredentials();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors(apiPolicy);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<InterceptionMiddleware>();
app.UseMiddleware<TokenCheckerMiddleware>();

app.UseOcelot().Wait();

app.Run();

using ApiGateway.Middlewares;
using Gateway.Middlewares;
using JwtAuthenticationManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SharedLibrary.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

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

app.UseCors(apiPolicy);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<InterceptionMiddleware>();
app.UseMiddleware<TokenCheckerMiddleware>();

app.UseOcelot().Wait();

app.Run();

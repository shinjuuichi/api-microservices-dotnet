using Gateway.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

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

app.UseMiddleware<InterceptionMiddleware>();

app.UseAuthorization();

app.UseOcelot().Wait();

app.Run();

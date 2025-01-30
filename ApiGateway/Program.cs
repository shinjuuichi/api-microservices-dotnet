using Gateway.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

var policy = "MicroservicesPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(policy, builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors(policy);

app.UseHttpsRedirection();

app.UseMiddleware<InterceptionMiddleware>();

app.UseAuthorization();

app.UseOcelot().Wait();

app.Run();

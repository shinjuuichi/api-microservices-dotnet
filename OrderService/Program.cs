using JwtAuthenticationManager;
using OrderService;
using SharedLibrary.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructureService();
builder.Services.AddWebAPIService();
builder.Services.AddCustomJwtAuthentication();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();


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

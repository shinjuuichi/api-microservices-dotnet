using ProductService;
using SharedLibrary;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureService();
builder.Services.AddWebAPIService();

builder.Services.AddControllers();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RestrictAccessMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

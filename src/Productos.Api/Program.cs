using Microsoft.EntityFrameworkCore;
using Productos.Api.Data;
using Productos.Api.Services;
var builder = WebApplication.CreateBuilder(args.Where(arg => arg != "--migrate").ToArray());
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ProductosDbContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("Productos") ?? throw new InvalidOperationException("Configura ConnectionStrings__Productos.")));
builder.Services.AddScoped<ProductosService>();
var app = builder.Build();
if (args.Contains("--migrate"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<ProductosDbContext>().Database.MigrateAsync();
    return;
}
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Productos v1"));
}
app.MapControllers();
app.Run();

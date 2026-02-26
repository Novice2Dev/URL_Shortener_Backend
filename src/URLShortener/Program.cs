using Microsoft.EntityFrameworkCore;
using URLShortener.Data;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

var envPath = Path.Combine("..", "..", ".env");
Env.Load(envPath);

var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION");
builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen();           // Now recognized

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

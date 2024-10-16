using SandboxService.API;
using SandboxService.Core.Interfaces;
using SandboxService.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<GlobalExceptionsMiddleware>();

builder.Services.AddSingleton<IMemoryCache, InMemoryCache>();
builder.Services.AddScoped<ICacheService, CacheService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionsMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

using FluentValidation;
using SandboxService.API;
using SandboxService.Application;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.Services;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Application.Validators;
using SandboxService.Core.Interfaces;
using SandboxService.Core.Models;
using SandboxService.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<GlobalExceptionsMiddleware>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddTransient<IValidator<SanboxInitializeRequest>, SandboxInitializeValidator>();

builder.Services.AddSingleton<IMemoryCache, InMemoryCache>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddScoped<IBinanceService, BinanceService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<SpotTradeService>();
builder.Services.AddScoped<MarginTradeService>();

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

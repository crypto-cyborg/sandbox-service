using System.Net.NetworkInformation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SandboxService.API;
using SandboxService.Application.Data.Dtos;
using SandboxService.Application.ServiceClients;
using SandboxService.Application.Services;
using SandboxService.Application.Services.Interfaces;
using SandboxService.Application.Validators;
using SandboxService.Persistence;
using SandboxService.Persistence.Contexts;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => 
{
    policyBuilder
        .WithOrigins(builder.Configuration["Client:BaseUrl"]!)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
}));

builder.Services.AddScoped<GlobalExceptionsMiddleware>();

builder.Services.AddDbContext<SandboxContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("ccdb-sandbox")));

builder.Services.AddScoped<UnitOfWork>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddTransient<IValidator<SanboxInitializeRequest>, SandboxInitializeValidator>();

builder.Services.AddScoped<UserServiceClient>();
builder.Services.AddHttpClient<UserServiceClient>(client => 
    client.BaseAddress = new Uri(builder.Configuration["InternalServices:UserService"]!));

builder.Services.AddScoped<ICurrencyService, CurrencyService>();

builder.Services.AddScoped<IBinanceService, BinanceService>();
builder.Services.AddHttpClient<IBinanceService, BinanceService>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Binance:BaseUrl"]!));

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddHttpClient<IAccountService, AccountService>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Binance:BaseUrl"]!));

builder.Services.AddScoped<SpotTradeService>();
builder.Services.AddScoped<MarginTradeService>();

var app = builder.Build();

// app.UseMiddleware<GlobalExceptionsMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
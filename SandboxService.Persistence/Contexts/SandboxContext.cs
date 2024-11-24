using System;
using Microsoft.EntityFrameworkCore;
using SandboxService.Core.Models;

namespace SandboxService.Persistence.Contexts;

public class SandboxContext(DbContextOptions<SandboxContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies { get; init; }
    public DbSet<Transaction> Transactions { get; init; }
    public DbSet<Account> Accounts { get; init; }
    public DbSet<Wallet> Wallets { get; init; }
    public DbSet<User> Users { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(currency =>
        {
            currency.HasData([
                new Currency { Id = 1, Name = "Tether", Ticker = "USDT" },
                new Currency { Id = 2, Name = "Bitcoin", Ticker = "BTC" }
            ]);
        });
    }
}
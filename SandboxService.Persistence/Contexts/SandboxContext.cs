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
            currency.Property(c => c.Id).IsRequired().HasDefaultValue(Guid.NewGuid());
            currency.HasData([
                new Currency { Name = "Tether", Ticker = "USDT" },
                new Currency { Name = "Bitcoin", Ticker = "BTC" }
            ]);
        });

        modelBuilder.Entity<Transaction>(transaction =>
        {
            transaction.Property(t => t.Id).IsRequired().HasDefaultValue(Guid.NewGuid());
            transaction.Property(t => t.Timestamp).HasDefaultValue(DateTimeOffset.UtcNow);
        });

        modelBuilder.Entity<Account>(account =>
        {
            account.Property(a => a.Id).IsRequired().HasDefaultValue(Guid.NewGuid());
            account.Property(a => a.Balance).IsRequired().HasDefaultValue(0);
        });

        modelBuilder.Entity<Wallet>(wallet =>
        {
            wallet.Property(w => w.Id).IsRequired().HasDefaultValue(Guid.NewGuid());
        });

        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.Id).IsRequired().HasDefaultValue(Guid.NewGuid());
        });
    }
}
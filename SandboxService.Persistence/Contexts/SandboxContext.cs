using System;
using Microsoft.EntityFrameworkCore;
using SandboxService.Core.Models;

namespace SandboxService.Persistence.Contexts;

public class SandboxContext(DbContextOptions<SandboxContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<MarginPosition> MarginPositions { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(currency =>
        {
            currency.HasData([
                new Currency { Id = 1, Name = "Tether", Ticker = "USDT" },
                new Currency { Id = 2, Name = "Bitcoin", Ticker = "BTC" }
            ]);
        });

        modelBuilder.Entity<User>()
            .Property(u => u.RowVersion)
            .IsRowVersion();
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<User>(u => u.WalletId);
        
        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasPrecision(18, 4);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 4);

    }
}
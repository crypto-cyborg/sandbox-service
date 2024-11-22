using System;
using Microsoft.EntityFrameworkCore;
using SandboxService.Core.Models;

namespace SandboxService.Persistence.Contexts;

public class SandboxContext(DbContextOptions<SandboxContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<User> Users { get; set; }
}

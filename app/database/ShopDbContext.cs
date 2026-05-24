using Microsoft.EntityFrameworkCore;
using bank.net.model;

namespace bank.net.database;

public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options)
{
    public DbSet<User> Users {get; set;}
    public DbSet<Card> Cards  {get; set;}
    public DbSet<Transfer> Transfers  {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация точности для денежных типов (decimal) в PostgreSQL
        modelBuilder.Entity<Card>()
            .Property(c => c.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transfer>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);
            
        base.OnModelCreating(modelBuilder);
    }
}
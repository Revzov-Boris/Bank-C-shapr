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
        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(c => c.Balance)
                .HasPrecision(12, 2);
            entity.Property(n => n.CardNumber)
                .HasMaxLength(16)
                .IsRequired();
            entity.HasIndex(c => c.CardNumber)
                .IsUnique();
            
        
        });
            
        modelBuilder.Entity<Transfer>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);
            
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        base.OnModelCreating(modelBuilder);
    }
}
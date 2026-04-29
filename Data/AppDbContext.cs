using LandOLakesDairyDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace LandOLakesDairyDemo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(product => product.ProductId).IsUnique();
            entity.HasIndex(product => product.Category);
            entity.HasIndex(product => product.ProductName);
            entity.HasIndex(product => product.IsFeatured);
            entity.Property(product => product.Price).HasPrecision(10, 2);
        });
    }
}
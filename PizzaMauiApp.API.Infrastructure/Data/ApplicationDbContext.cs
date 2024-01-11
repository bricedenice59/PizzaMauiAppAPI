using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.Models;

namespace PizzaMauiApp.API.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<PizzaProductImage> PizzaProductImages { get; set; }
    public DbSet<PizzaProduct> PizzaProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PizzaProductImage>()
            .HasOne<PizzaProduct>()
            .WithMany(p=>p.ProductImages)
            .HasForeignKey(p => p.ProductId);
    }
}
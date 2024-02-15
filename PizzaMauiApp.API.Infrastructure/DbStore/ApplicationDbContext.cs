using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.Models;

namespace PizzaMauiApp.API.Infrastructure.DbStore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<PizzaProductImage> PizzaProductImages { get; set; }
    public DbSet<PizzaProduct> PizzaProducts { get; set; }
    
    public DbSet<Order> PizzaOrders { get; set; }
    
    public DbSet<OrderStatusUpdate> PizzaOrdersStatusHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PizzaProductImage>()
            .HasOne<PizzaProduct>()
            .WithMany(p=>p.ProductImages)
            .HasForeignKey(p => p.ProductId);
        
        modelBuilder.Entity<OrderItems>()
            .HasOne<Order>()
            .WithMany(p=>p.OrderItems)
            .HasForeignKey(p => p.OrderId)
            .IsRequired(false);
        
        modelBuilder.Entity<OrderStatusUpdate>()
            .HasOne<Order>()
            .WithMany(p=>p.OrdersStatusHistory)
            .HasForeignKey(p => p.OrderId)
            .IsRequired(false);
    }
}
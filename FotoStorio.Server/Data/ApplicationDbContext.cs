using System;
using FotoStorio.Shared.Models;
using FotoStorio.Shared.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace FotoStorio.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            // defines the Address as being owned by an Order
            modelBuilder.Entity<Order>()
                .OwnsOne(o => o.SendToAddress, a => {
                    a.WithOwner();
                });

            // establishes that OrderItems are deleted if an Order is deleted
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // defines the ItemOrdered as being owned by an OrderItem
            modelBuilder.Entity<OrderItem>()
                .OwnsOne(oi => oi.ItemOrdered, i => {
                    i.WithOwner();
                });

            // converts the OrderStatus enum into a string
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion(
                    os => os.ToString(),
                    os => (OrderStatus) Enum.Parse(typeof(OrderStatus), os)
                );
        }
    }
}
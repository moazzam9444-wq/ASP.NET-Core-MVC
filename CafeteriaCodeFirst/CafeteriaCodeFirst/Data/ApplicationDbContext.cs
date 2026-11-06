using Microsoft.EntityFrameworkCore;
using CafeteriaCodeFirst.Models;

namespace CafeteriaCodeFirst.Data
{
    public class ApplicationDbContext : DbContext
    {
        
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

       
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(x => x.PurchaseOrder)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
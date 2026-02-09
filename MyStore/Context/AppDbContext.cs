using Microsoft.EntityFrameworkCore;
using MyStore.Entities;
 


namespace MyStore.Context
{
    public class AppDbContext : DbContext
    {
        //base:las opciones que hemos configurado sean enviadas a DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey("CategoryId");
                e.Property("CategoryId").ValueGeneratedOnAdd();
                e.HasData(
                    new Category { CategoryId = 1, Name = "Technology"},
                    new Category { CategoryId = 2, Name = "Bedroom"}


                    );
            });
            //uno a muchos
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey("ProductId");
                e.Property("ProductId").ValueGeneratedOnAdd();
                e.Property("Price").HasColumnType("decimal(10,2)");
                e.HasOne(e => e.Category).WithMany(p => p.Products).HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey("UserId");
                e.Property("UserId").ValueGeneratedOnAdd();
            });
            //uno a muchos
            modelBuilder.Entity<Order>(e =>
            {
                e.HasKey("OrderId");
                e.Property("OrderId").ValueGeneratedOnAdd();
                e.Property("TotalAmount").HasColumnType("decimal(10,2)");
                e.HasOne(e => e.User).WithMany(p => p.Orders).HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            //una orden puede tener muchos orderItems
            modelBuilder.Entity<OrderItem>(e =>
            {
                e.HasKey("OrderItemId");
                e.Property("OrderItemId").ValueGeneratedOnAdd();
                e.Property("Price").HasColumnType("decimal(10,2)");
                e.HasOne(e => e.Order).WithMany(p => p.OrderItems).HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
                //por cada orderItem tengo un producto relacion uno a uno
                e.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });

        }

    }
}
//en el package manager console comenzamos con Add-Migration firstMigration,para remover migracion Remove-Migration
//luego para crear la base de datos del conectionString Update-Database
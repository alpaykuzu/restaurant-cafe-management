using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<SalesReport> SalesReports { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 DECIMAL ALANLAR
            modelBuilder.Entity<MenuItem>().Property(m => m.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesReport>().Property(sr => sr.TotalSales).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesReport>().Property(sr => sr.AverageOrderValue).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Employee>().Property(e => e.Salary).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<InventoryItem>().Property(ii => ii.Cost).HasColumnType("decimal(18,2)");

            // 🔹 RESTAURANT İLİŞKİLERİ
            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Categories)
                .WithOne(c => c.Restaurant)
                .HasForeignKey(c => c.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Employees)
                .WithOne(e => e.Restaurant)
                .HasForeignKey(e => e.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.InventoryItems)
                .WithOne(ii => ii.Restaurant)
                .HasForeignKey(ii => ii.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.MenuItems)
                .WithOne(mi => mi.Restaurant)
                .HasForeignKey(mi => mi.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Orders)
                .WithOne(o => o.Restaurant)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Tables)
                .WithOne(t => t.Restaurant)
                .HasForeignKey(t => t.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.SalesReports)
                .WithOne(sr => sr.Restaurant)
                .HasForeignKey(sr => sr.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.Reservations)
                .WithOne(res => res.Restaurant)
                .HasForeignKey(res => res.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 CATEGORY → MENUITEM
            modelBuilder.Entity<Category>()
                .HasMany(c => c.MenuItems)
                .WithOne(mi => mi.Category)
                .HasForeignKey(mi => mi.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 USER İLİŞKİLERİ
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 EMPLOYEE İLİŞKİLERİ
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.InventoryTransactions)
                .WithOne(it => it.Employee)
                .HasForeignKey(it => it.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Orders)
                .WithOne(o => o.Employee)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 INVENTORY İLİŞKİLERİ
            modelBuilder.Entity<InventoryItem>()
                .HasMany(ii => ii.InventoryTransactions)
                .WithOne(it => it.InventoryItem)
                .HasForeignKey(it => it.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 ORDER İLİŞKİLERİ
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Invoice)
                .WithOne(i => i.Order)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 TABLE İLİŞKİLERİ
            modelBuilder.Entity<Table>()
                .HasMany(t => t.Orders)
                .WithOne(o => o.Table)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Table>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Table)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 MENUITEM İLİŞKİLERİ
            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.OrderItems)
                .WithOne(oi => oi.MenuItem)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 PERFORMANS İÇİN INDEX'LER
            modelBuilder.Entity<MenuItem>()
                .HasIndex(mi => mi.CategoryId)
                .HasDatabaseName("IX_MenuItem_CategoryId");

            modelBuilder.Entity<MenuItem>()
                .HasIndex(mi => mi.RestaurantId)
                .HasDatabaseName("IX_MenuItem_RestaurantId");

            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => oi.OrderId)
                .HasDatabaseName("IX_OrderItem_OrderId");

            modelBuilder.Entity<OrderItem>()
                .HasIndex(oi => oi.MenuItemId)
                .HasDatabaseName("IX_OrderItem_MenuItemId");

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.RestaurantId)
                .HasDatabaseName("IX_Employee_RestaurantId");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.RestaurantId)
                .HasDatabaseName("IX_Order_RestaurantId");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.TableId)
                .HasDatabaseName("IX_Order_TableId");

        }
    }
}
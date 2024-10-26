using BusinessObject;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>().HasKey(o => o.OrderId);

        builder.Entity<Order>()
            .HasOne(x => x.IdentityUser)
            .WithMany()
            .HasForeignKey(o => o.MemberId);

        builder.Entity<OrderDetail>()
            .HasKey(od => new { od.OrderId, od.ProductId });

        builder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        builder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId);

        builder.Entity<Product>().HasKey(p => p.ProductId);
        builder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        builder.Entity<Category>().HasKey(c => c.CategoryId);


        builder.Entity<Category>().HasData(
      new Category { CategoryId = 1, CategoryName = "Beverages" },
      new Category { CategoryId = 2, CategoryName = "Food" }
  );

        builder.Entity<Product>().HasData(
            new Product { ProductId = 1, CategoryId = 1, ProductName = "Coffee", Weight = 12, UnitPrice = 12, UnitsInStock = 100 },
            new Product { ProductId = 2, CategoryId = 1, ProductName = "Tea", Weight = 12, UnitPrice = 5, UnitsInStock = 150 },
            new Product { ProductId = 3, CategoryId = 2, ProductName = "Bread", Weight = 12, UnitPrice = 2, UnitsInStock = 200 }
        );

    }
}

using Microsoft.EntityFrameworkCore;
using WebAspDBeaverStudy.Data.Entities;

namespace WebAspDBeaverStudy.Data
{
    public class AppDbContext : DbContext // Клас AppDbContext успадковується від DbContext
    {
        // Конструктор, який приймає параметри конфігурації для DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Властивості для роботи з категоріями, продуктами та зображеннями
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductsImages { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using WebAspDBeaverStudy.Data.Entities;

namespace WebAspDBeaverStudy.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<CategoryEntity> Categories { get; set; }
    }
}

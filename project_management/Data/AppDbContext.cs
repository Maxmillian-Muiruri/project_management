using Microsoft.EntityFrameworkCore;
using project_management.Models;

namespace project_management.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
    }
}

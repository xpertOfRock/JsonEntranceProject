using Json.Models;
using Microsoft.EntityFrameworkCore;

namespace Json.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Item> Items { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;

namespace NjianiAPI.Data
{
    public class NjianiDbContext : DbContext
    {
        public NjianiDbContext(DbContextOptions<NjianiDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
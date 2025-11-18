using Microsoft.EntityFrameworkCore;
using NjianiAPI.Models;

namespace NjianiAPI.Data
{
    public class NjianiDbContext : DbContext
    {
        public NjianiDbContext(DbContextOptions<NjianiDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ClaimT> Claims { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ClaimImage> ClaimImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClaimT>()
                .HasMany(c => c.Images)
                .WithOne(i => i.Claim)
                .HasForeignKey(i => i.ClaimId);

            modelBuilder.Entity<Location>()
                .HasMany(l => l.Claims)
                .WithOne(c => c.Location)
                .HasForeignKey(c => c.LocationId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Claims)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);
        }
    }
}
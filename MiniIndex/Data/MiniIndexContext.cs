using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MiniIndex.Models
{
    public class MiniIndexContext : IdentityDbContext
    {
        public MiniIndexContext(DbContextOptions<MiniIndexContext> options)
            : base(options)
        {
        }

        public DbSet<Mini> Mini { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<MiniTag> MiniTag { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
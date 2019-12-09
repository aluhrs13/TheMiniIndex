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

        public DbSet<MiniIndex.Models.Mini> Mini { get; set; }
        public DbSet<MiniIndex.Models.Tag> Tag { get; set; }
        public DbSet<MiniIndex.Models.Creator> Creator { get; set; }
        public DbSet<MiniIndex.Models.MiniTag> MiniTag { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
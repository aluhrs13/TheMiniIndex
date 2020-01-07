using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Persistence
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
        public DbSet<Starred> Starred { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
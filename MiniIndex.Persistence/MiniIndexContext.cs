using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniIndex.Models;

namespace MiniIndex.Persistence
{
    public class MiniIndexContext : ApiAuthorizationDbContext<IdentityUser>
    {
        public MiniIndexContext(DbContextOptions<MiniIndexContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Mini> Mini { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<MiniTag> MiniTag { get; set; }
        public DbSet<Starred> Starred { get; set; }
        public DbSet<TagPair> TagPair { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
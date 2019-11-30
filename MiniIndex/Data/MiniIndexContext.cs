using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniIndex.Models;

namespace MiniIndex.Models
{
    public class MiniIndexContext : IdentityDbContext
    {
        public MiniIndexContext (DbContextOptions<MiniIndexContext> options)
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

            modelBuilder.Entity<MiniTag>()
                .HasKey(t => new { t.MiniID, t.TagID });

            modelBuilder.Entity<MiniTag>()
                .HasOne(pt => pt.Mini)
                .WithMany(p => p.MiniTags)
                .HasForeignKey(pt => pt.MiniID);

            modelBuilder.Entity<MiniTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.MiniTags)
                .HasForeignKey(pt => pt.TagID);
        }
    }
}

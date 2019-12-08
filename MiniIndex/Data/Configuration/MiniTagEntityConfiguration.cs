using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniIndex.Models;

namespace MiniIndex.Data.Configuration
{
    public class MiniTagEntityConfiguration : IEntityTypeConfiguration<MiniTag>
    {
        public void Configure(EntityTypeBuilder<MiniTag> builder)
        {
            builder.HasKey(t => new { t.MiniID, t.TagID });

            builder.HasOne(pt => pt.Mini)
                .WithMany(p => p.MiniTags)
                .HasForeignKey(pt => pt.MiniID);

            builder.HasOne(pt => pt.Tag)
                .WithMany(t => t.MiniTags)
                .HasForeignKey(pt => pt.TagID);
        }
    }
}
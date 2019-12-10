using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniIndex.Models;

namespace MiniIndex.Data.Configuration
{
    public class CreatorEntityConfiguration : IEntityTypeConfiguration<Creator>
    {
        public void Configure(EntityTypeBuilder<Creator> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasMany(x => x.Sites)
                .WithOne(x => x.Creator);
        }
    }
}
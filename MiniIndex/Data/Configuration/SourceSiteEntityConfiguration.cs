using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;

namespace MiniIndex.Data.Configuration
{
    public class SourceSiteEntityConfiguration : IEntityTypeConfiguration<SourceSite>
    {
        public void Configure(EntityTypeBuilder<SourceSite> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasDiscriminator(x => x.DisplayName)
                .HasValue<ThingiverseSource>("Thingiverse");

            builder.Ignore(x => x.BaseUri);
            builder.Ignore(x => x.CreatorPageUri);
        }
    }
}
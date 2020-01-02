using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniIndex.Models.SourceSites;

namespace MiniIndex.Persistence.Configuration
{
    public class MiniSourceSiteEntityConfiguration : IEntityTypeConfiguration<MiniSourceSite>
    {
        public void Configure(EntityTypeBuilder<MiniSourceSite> builder)
        {
            builder.HasKey(x => x.ID);
        }
    }
}
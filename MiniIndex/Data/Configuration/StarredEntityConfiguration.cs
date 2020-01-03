using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniIndex.Models;

namespace MiniIndex.Data.Configuration
{
    public class StarredEntityConfiguration : IEntityTypeConfiguration<Starred>
    {
        public void Configure(EntityTypeBuilder<Starred> builder)
        {
            builder.HasKey(s => new { s.MiniID, s.UserID });
        }
    }
}
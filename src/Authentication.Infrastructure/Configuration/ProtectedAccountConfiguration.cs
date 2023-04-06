using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Infrastructure.Configuration;

public class ProtectedAccountConfiguration : IEntityTypeConfiguration<ProtectedAccount>
{
    public void Configure(EntityTypeBuilder<ProtectedAccount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Infrastructure.Configuration;

public class GuestAccountConfiguration : IEntityTypeConfiguration<GuestAccount>
{
    public void Configure(EntityTypeBuilder<GuestAccount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.DeviceId).IsUnique();
    }
}

namespace server.data.configurations;

using core.models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DbUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(u => u.Password)
            .IsRequired();

        builder.HasIndex(u => u.Username)
            .IsUnique();
    }
}

namespace server.data.configurations;

using core.models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DbTaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(t => t.Body)
            .HasMaxLength(2500);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .HasDefaultValue(null);

        builder.Property(t => t.IsEdited)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Categories)
               .WithMany(c => c.Tasks)
               .UsingEntity(
                   "task_categories",
                   l => l.HasOne(typeof(Category)).WithMany().HasForeignKey("category_id"),
                   r => r.HasOne(typeof(Task)).WithMany().HasForeignKey("task_id")
               );

        builder.HasIndex(t => t.UserId);
    }
}

namespace server.data;

using core.models;
using server.data.configurations;

using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DbUserConfiguration());
        modelBuilder.ApplyConfiguration(new DbTaskConfiguration());
        modelBuilder.ApplyConfiguration(new DbCategoryConfiguration());
    }
}

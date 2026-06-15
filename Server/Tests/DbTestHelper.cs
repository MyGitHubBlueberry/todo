using Server.Data;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Server.Tests;

public static class DbTestHelper
{
    public static async
        Task<(SqliteConnection, DbContextOptions<AppDbContext>, IConfiguration)>
        SetupTestDbAsync()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        var inMemorySettings = new Dictionary<string, string?> {
            {"JwtSettings:SecretKey", "SuperSecretTestKeyThatIsAtLeast32CharsLong123!"},
            {"JwtSettings:Issuer", "TestIssuer"},
            {"JwtSettings:Audience", "TestAudience"}
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        return (connection, options, config);
    }
}

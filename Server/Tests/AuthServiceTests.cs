using Server.Data;
using Server.Services;
using Server.Core.Models;
using Server.Core.Dtos.Auth;

using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Server.Tests.Services;

[Trait("Service", "AuthService")]
public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_Successfully_Creates_User_And_Hashes_Password()
    {
        var (connection, options, config) = await DbTestHelper.SetupTestDbAsync();

        var dto = new RegistrationDto("testuser_1", "MySecretPassword123");

        using (var context = new AppDbContext(options))
        {
            var authService = new AuthService(context, config);
            var tokens = await authService.RegisterAsync(dto);

            Assert.NotNull(tokens.accessToken);
            Assert.NotNull(tokens.refreshToken);
        }

        using (var context = new AppDbContext(options))
        {
            var savedUser = await context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.login);

            Assert.NotNull(savedUser);
            Assert.Equal(dto.login, savedUser.Username);

            Assert.NotEqual(dto.password, savedUser.Password);
            Assert.True(BCrypt.Net.BCrypt.Verify(dto.password, savedUser.Password));
        }
    }

    [Fact]
    public async Task LoginAsync_Successfully_Logs_In_And_Generates_Tokens()
    {
        var (connection, options, config) = await DbTestHelper.SetupTestDbAsync();

        var dto = new LoginDto("testuser_1", "MySecretPassword123");

        using (var context = new AppDbContext(options))
        {
            await context.Users.AddAsync(new User
            {
                Username = dto.login,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.password),
            });
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var authService = new AuthService(context, config);
            var tokens = await authService.LoginAsync(dto);

            Assert.NotNull(tokens.accessToken);
            Assert.NotNull(tokens.refreshToken);
        }

        using (var context = new AppDbContext(options))
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.login);

            Assert.NotNull(user);
            Assert.Equal(dto.login, user.Username);

            Assert.NotEqual(dto.password, user.Password);
            Assert.True(BCrypt.Net.BCrypt.Verify(dto.password, user.Password));
        }
    }
}

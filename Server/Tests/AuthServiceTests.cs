using Server.Data;
using Server.Services;
using Server.Core.Dtos.Auth;

using Microsoft.EntityFrameworkCore;

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
}

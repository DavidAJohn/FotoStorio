using FotoStorio.Shared.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FotoStorio.Server.Services;

public class TokenServiceTests
{
    private readonly IConfiguration _config;
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly AppUser _user;
    private readonly List<string> _roles = ["Admin"];

    public TokenServiceTests()
    {
        _config = Substitute.For<IConfiguration>();
        _config["JwtKey"].Returns("£MockJwtKeyMockJwtKeyMockJwtKey$_£MockJwtKeyMockJwtKeyMockJwtKey$");
        _config["JwtIssuer"].Returns("MockJwtIssuer");
        _config["JwtAudience"].Returns("MockJwtAudience");
        _config["JwtExpiryInDays"].Returns("1");

        _user = new AppUser { Email = "test@test.com", DisplayName = "Test User" };

        var store = Substitute.For<IUserStore<AppUser>>();
        _userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        _tokenService = new TokenService(_config, _userManager);
    }

    [Fact]
    public async Task CreateToken_ShouldReturnValidToken()
    {
        // Arrange
        _userManager.GetRolesAsync(_user).Returns(_roles);

        // Act
        var token = await _tokenService.CreateToken(_user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().BeOfType<string>();
    }
}

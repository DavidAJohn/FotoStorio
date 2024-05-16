using FotoStorio.Shared.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

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

    [Fact]
    public async Task CreateToken_ShouldReturnTokenWithClaims()
    {
        // Arrange
        _userManager.GetRolesAsync(_user).Returns(_roles);

        // Act
        var token = await _tokenService.CreateToken(_user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        securityToken.Should().NotBeNull();
        securityToken.Issuer.Should().Be(_config["JwtIssuer"]);
        securityToken.Audiences.Should().Contain(_config["JwtAudience"]);
        securityToken.Claims.Should().NotBeEmpty();
        securityToken.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == _user.Email);
        securityToken.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.GivenName && claim.Value == _user.DisplayName);
    }
}

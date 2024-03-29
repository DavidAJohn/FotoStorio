using AutoMapper;
using FotoStorio.Shared.Auth;
using FotoStorio.Shared.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FotoStorio.Server.Controllers;

public class AccountsControllerTests
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppUser _user;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountsController> _logger;
    private readonly AccountsController _accountsController;

    public AccountsControllerTests()
    {
        _user = new AppUser { Email = "test@test.com", DisplayName = "Test User" };

        var store = Substitute.For<IUserStore<AppUser>>();
        _userManager = Substitute.For<UserManager<AppUser>>(store, null, null, null, null, null, null, null, null);
        _signInManager = Substitute.For<SignInManager<AppUser>>(_userManager, Substitute.For<IHttpContextAccessor>(), Substitute.For<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null, null);

        _tokenService = Substitute.For<ITokenService>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<AccountsController>>();

        _accountsController = new AccountsController(
            _userManager,
            _signInManager,
            _tokenService,
            _mapper,
            _logger
        );
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserIsNull()
    {
        // Arrange
        var loginModel = new LoginModel { Email = "test@test.com", Password = "Test1234" };
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((AppUser)null);

        // Act
        var result = await _accountsController.Login(loginModel);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenSignInFails()
    {
        // Arrange
        var loginModel = new LoginModel { Email = "test@test.com", Password = "" };
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);

        _signInManager.CheckPasswordSignInAsync(_user, Arg.Any<string>(), Arg.Any<bool>())
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = (BadRequestObjectResult)await _accountsController.Login(loginModel);

        // Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeOfType<LoginResult>();
        ((LoginResult)result.Value).Successful.Should().BeFalse();
        ((LoginResult)result.Value).Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenSignInSucceeds()
    {
        // Arrange
        var test_jwt_token = "testtoken";

        var loginModel = new LoginModel { Email = "test@test.com", Password = "Test1234" };
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);

        _signInManager.CheckPasswordSignInAsync(_user, Arg.Any<string>(), Arg.Any<bool>())
            .Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _tokenService.CreateToken(_user).Returns(test_jwt_token);

        // Act
        var result = (OkObjectResult)await _accountsController.Login(loginModel);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<LoginResult>();
        ((LoginResult)result.Value).Successful.Should().BeTrue();
        ((LoginResult)result.Value).Token.Should().Be(test_jwt_token);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenEmailAddressAlreadyExists()
    {
        // Arrange
        var registerModel = new RegisterModel { 
            DisplayName = "Test User", 
            Email = "test@test.com", 
            Password = "Test1234", 
            ConfirmPassword = "Test1234" 
        };

        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(_user);

        // Act
        var result = (BadRequestObjectResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeOfType<RegisterResult>();
        ((RegisterResult)result.Value).Successful.Should().BeFalse();
        //((RegisterResult)result.Value).Error.Should().Be("The email address is already in use");
        ((RegisterResult)result.Value).Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserCouldNotBeCreated()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1234"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password)
            .Returns(IdentityResult.Failed());

        // Act
        var result = (BadRequestResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenRoleCouldNotBeAdded()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1234"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password)
            .Returns(IdentityResult.Success);

        _userManager.AddToRoleAsync(Arg.Any<AppUser>(), "User")
            .Returns(IdentityResult.Failed());

        // Act
        var result = (BadRequestResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenPasswordIsEmpty()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "",
            ConfirmPassword = "Test1234"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password).Returns(IdentityResult.Failed());

        // Act
        var result = (BadRequestObjectResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeOfType<RegisterResult>();
        ((RegisterResult)result.Value).Successful.Should().BeFalse();
        //((RegisterResult)result.Value).Error.Should().Be("Password fields must be complete and matching");
        ((RegisterResult)result.Value).Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1235"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password).Returns(IdentityResult.Failed());

        // Act
        var result = (BadRequestObjectResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeOfType<RegisterResult>();
        ((RegisterResult)result.Value).Successful.Should().BeFalse();
        //((RegisterResult)result.Value).Error.Should().Be("Passwords do not match");
        ((RegisterResult)result.Value).Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            DisplayName = "Test User",
            Email = "test@test.com",
            Password = "Test1234",
            ConfirmPassword = "Test1234"
        };

        _userManager.CreateAsync(Arg.Any<AppUser>(), registerModel.Password)
            .Returns(IdentityResult.Success);

        _userManager.AddToRoleAsync(Arg.Any<AppUser>(), "User")
            .Returns(IdentityResult.Success);

        // Act
        var result = (OkObjectResult)await _accountsController.Register(registerModel);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<RegisterResult>();
        ((RegisterResult)result.Value).Successful.Should().BeTrue();
    }
}

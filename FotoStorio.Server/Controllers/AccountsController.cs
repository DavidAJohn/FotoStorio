using AutoMapper;
using FotoStorio.Server.Contracts;
using FotoStorio.Server.Extensions;
using FotoStorio.Shared.Auth;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FotoStorio.Server.Controllers;

public class AccountsController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountsController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserManagerExtensionsWrapper _userManagerExtensionsWrapper;

    public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper, ILogger<AccountsController> logger, IHttpContextAccessor httpContextAccessor, IUserManagerExtensionsWrapper userManagerExtensionsWrapper)
    {
        _logger = logger;
        _mapper = mapper;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _userManagerExtensionsWrapper = userManagerExtensionsWrapper;
    }

    /// GET api/accounts/login
    /// <summary>
    /// Authenticates a user
    /// </summary>
    /// <returns>LoginResult object</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login(LoginModel login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);

        if (user == null)
        {
            _logger.LogInformation("AccountsController.Login: User '{email}' not found", login.Email);
            return Unauthorized();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

        if (!result.Succeeded)
        {
            _logger.LogInformation("AccountsController.Login: Login failed for user '{email}'", login.Email);
            return BadRequest(new LoginResult { Successful = false, Error = "Login failed" });
        }

        return Ok(
            new LoginResult
            {
                Successful = true,
                Token = await _tokenService.CreateToken(user)
            }
        ); 
    }

    /// GET api/accounts/register
    /// <summary>
    /// Creates a new user account
    /// </summary>
    /// <returns>RegisterResult object</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register(RegisterModel register)
    {
        if (string.IsNullOrWhiteSpace(register.Password) || string.IsNullOrWhiteSpace(register.ConfirmPassword))
        {
            _logger.LogWarning("AccountsController.Register: Registration failed because Password or ConfirmPassword were empty");

            return new BadRequestObjectResult(
                new RegisterResult
                {
                    Successful = false,
                    Error = "Password fields must be complete and matching"
                }
            );
        }

        if (register.Password != register.ConfirmPassword) 
        {
            _logger.LogWarning("AccountsController.Register: Registration failed because Password and ConfirmPassword did not match");

            return new BadRequestObjectResult(
                new RegisterResult
                {
                    Successful = false,
                    Error = "Passwords do not match"
                }
            );
        }

        if (await CheckEmailExistsAsync(register.Email))
        {
            _logger.LogWarning("AccountsController.Register: Registration failed because email address '{email}' was already in use", register.Email);

            return new BadRequestObjectResult(
                new RegisterResult{
                    Successful = false,
                    Error = "The email address is already in use"
                }
            );
        }

        var user = new AppUser
        {
            DisplayName = register.DisplayName,
            Email = register.Email,
            UserName = register.Email
        };

        var result = await _userManager.CreateAsync(user, register.Password);

        if (!result.Succeeded)
        {
            _logger.LogWarning("AccountsController.Register: Registration failed because the user '{email}' could not be created by the UserManager", user.Email);
            return BadRequest();
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            _logger.LogWarning("AccountsController.Register: Registration failed because the role 'User' could not be added to the user '{email}' by the UserManager", user.Email);
            return BadRequest();
        }

        _logger.LogInformation("AccountsController.Register: Registration for user '{email}' was completed successfully", user.Email);

        return Ok(
            new RegisterResult
            {
                Successful = true
            }
        );
    }

    private async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    /// GET api/accounts/address
    /// <summary>
    /// Returns the authenticated user's default address
    /// </summary>
    /// <returns>AddressDTO object</returns>
    [Authorize]
    [HttpGet("address")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserAddress()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = await _userManagerExtensionsWrapper.FindUserByClaimsPrincipalWithAddressAsync(_userManager, httpContext.User);

            if (user == null)
            {
                return new NotFoundObjectResult(
                    new AddressDTO { }
                );
            }

            if (user.Address == null)
            {
                return Ok(new AddressDTO { });
            }

            return Ok(_mapper.Map<Address, AddressDTO>(user.Address));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in AccountsController.GetUserAddress: {message}", ex.Message);

            return new NotFoundObjectResult(
                new AddressDTO { }
            );
        }
    }

    /// PUT api/accounts/address
    /// <summary>
    /// Updates the authenticated user's default address
    /// </summary>
    /// <returns>AddressDTO object</returns>
    [Authorize]
    [HttpPut("address")]
    public async Task<ActionResult> UpdateUserAddress(AddressDTO addressDTO)
    {   
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            AppUser user = await _userManagerExtensionsWrapper.FindUserByClaimsPrincipalWithAddressAsync(_userManager, httpContext.User);

            user.Address = _mapper.Map<AddressDTO, Address>(addressDTO);
            
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(_mapper.Map<Address, AddressDTO>(user.Address));
            }

            return Ok(new AddressDTO {});
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in AccountsController.UpdateUserAddress : {message}", ex.Message);

            return Ok(new AddressDTO { });
        }
    }
}
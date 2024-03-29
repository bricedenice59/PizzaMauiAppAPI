using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.Environment;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Core.Services;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;
using PizzaMauiApp.API.Models.Identity;

namespace PizzaMauiApp.API.Controllers;

public class AccountsController : BaseApiController
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly TokenAuth0Config _tokenConfig;
    
    public AccountsController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IMapper mapper,
        TokenAuth0Config tokenConfig)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _tokenConfig = tokenConfig;
    }
    

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ApiResponse<UserIdentityDto>> Login(UserLoginDto loginData)
    {
        var user = await _userManager.FindByEmailAsync(loginData.Email);
        if (user is null)
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status404NotFound, "User not found.");

        var loginResult = await _signInManager.CheckPasswordSignInAsync(user, loginData.Password, true);
        if(!loginResult.Succeeded)
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status404NotFound, "User found but password does not match");

        var token = _tokenService.CreateToken(new TokenUser { Email = user!.Email, Name = user.DisplayName });
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.ToUniversalTime().AddDays(_tokenConfig.RefreshTokenValidityInDays);
        
        await _userManager.UpdateAsync(user);
        
        var userIdentityDto = 
            new UserIdentityDto
            {
                Id = new Guid(user.Id),
                Email = loginData.Email, 
                FirstName = user.DisplayName, 
                Token = token,
                RefreshToken = refreshToken
            };
        
        return new ApiResponse<UserIdentityDto>(StatusCodes.Status200OK, userIdentityDto);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ApiResponse<UserIdentityDto>> Register(UserRegisterDto registerData)
    {
        if(string.IsNullOrEmpty(registerData.Email))
            return new ApiResponse<UserIdentityDto> (StatusCodes.Status400BadRequest,"Email address cannot be null or empty");
        
        if (await _userManager.FindByEmailAsync(registerData.Email) != null)
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status400BadRequest,"Email address is in use");

        //create a random username for now
        Random rand = new Random();
        var username = registerData.Email.Split('@')[0] +"_"+ rand.Next(0, 1_000_000);
        var user = new User
        {
            Email = registerData.Email,
            DisplayName = username,
            UserName = username
        };
        
        var token = _tokenService.CreateToken(new TokenUser { Email = user.Email, Name = user.DisplayName });
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.ToUniversalTime().AddDays(_tokenConfig.RefreshTokenValidityInDays);

        var registerResult = await _userManager.CreateAsync(user, registerData.Password);
        if (!registerResult.Succeeded)
        {
            StringBuilder sb = new();
            foreach (var error in registerResult.Errors)
            {
                sb.AppendLine($"{error.Code}, {error.Description}");
            }
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status400BadRequest,sb.ToString());
        }
   
        var userIdentityDto = new UserIdentityDto
        {
            Id = new Guid(user.Id),
            Email = registerData.Email,
            FirstName = user.DisplayName, 
            Token = token,
            RefreshToken = refreshToken
        };
        return new ApiResponse<UserIdentityDto>(StatusCodes.Status200OK, userIdentityDto);
    }
    
    [API.Attributes.Authorize]
    [HttpGet]
    public ActionResult<UserIdentityDto> GetCurrentUser()
    {
        var context = HttpContext;
        var user = context.Items["User"] as TokenUser;

        return new UserIdentityDto
        {
            Email = user!.Email,
            Token = _tokenService.CreateToken(new TokenUser{Email = user!.Email, Name = user.Name})
        };
    }
    
    [HttpGet("emailexists")]
    public async Task<ApiResponse<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
        var userFound = await _userManager.FindByEmailAsync(email) != null;
        return userFound 
            ? new ApiResponse<bool>(StatusCodes.Status200OK, userFound) 
            : new ApiResponse<bool>(StatusCodes.Status404NotFound, "User not found");
    }

    [API.Attributes.Authorize]
    [HttpGet("address")]
    public async Task<ApiResponse<UserAddressDto>> GetUserAddressById(string id)
    {
        var user =  await _userManager.Users.Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Id == id);

        if(user is null)
            return new ApiResponse<UserAddressDto>(StatusCodes.Status404NotFound, "User not found");
        
        return new ApiResponse<UserAddressDto>(StatusCodes.Status200OK,_mapper.Map<UserAddress, UserAddressDto>(user.Address));
    }

    [API.Attributes.Authorize]
    [HttpPut("address")]
    public async Task<ApiResponse<UserAddressDto>> UpdateUserAddress(UserAddressDto address)
    {
        var context = HttpContext;
        var userFromContext = context.Items["User"] as TokenUser;
        var user = await _userManager.Users.Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Email == userFromContext.Email);
        
        if(user is null)
            return new ApiResponse<UserAddressDto>(StatusCodes.Status404NotFound, "User not found");
        
        user.Address = _mapper.Map<UserAddressDto, UserAddress>(address);

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded 
            ? new ApiResponse<UserAddressDto>(StatusCodes.Status200OK,_mapper.Map<UserAddressDto>(user.Address)) 
            : new ApiResponse<UserAddressDto>(StatusCodes.Status400BadRequest, "Problem updating the user");
    }
}
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Core.Models.Identity;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;

namespace PizzaMauiApp.API.Controllers;

public class AccountsController : BaseApiController
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    
    public AccountsController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ApiResponse<UserIdentityDto>> Login(UserLoginDto loginData)
    {
        var user = await _userManager.FindByEmailAsync(loginData.Email);
        if (user == null)
            return new ApiResponse<UserIdentityDto>(401, "User not found.");

        var loginResult = await _signInManager.CheckPasswordSignInAsync(user, loginData.Password, true);
        if(!loginResult.Succeeded)
            return new ApiResponse<UserIdentityDto>(401, "User found but password does not match");

        var userIdentityDto = 
            new UserIdentityDto
            {
                Id = new Guid(user.Id),
                Email = loginData.Email, 
                FirstName = user.DisplayName, 
                Token = _tokenService.CreateToken(user)
            };
        
        return new ApiResponse<UserIdentityDto>(200, userIdentityDto);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ApiResponse<UserIdentityDto>> Register(UserRegisterDto registerData)
    {
        if(string.IsNullOrEmpty(registerData.Email))
            return new ApiResponse<UserIdentityDto> (400,"Email address cannot be null or empty");
        
        if (await _userManager.FindByEmailAsync(registerData.Email) != null)
            return new ApiResponse<UserIdentityDto>(400,"Email address is in use");

        //create a random username for now
        Random rand = new Random();
        var username = registerData.Email.Split('@')[0] +"_"+ rand.Next(0, 1_000_000);
        var user = new User
        {
            Email = registerData.Email,
            DisplayName = username,
            UserName = username
        };

        var registerResult = await _userManager.CreateAsync(user, registerData.Password);
        if (!registerResult.Succeeded)
        {
            StringBuilder sb = new();
            foreach (var error in registerResult.Errors)
            {
                sb.AppendLine($"{error.Code}, {error.Description}");
            }
            return new ApiResponse<UserIdentityDto>(400,sb.ToString());
        }
   
        var userIdentityDto = new UserIdentityDto
        {
            Id = new Guid(user.Id),
            Email = registerData.Email,
            FirstName = user.DisplayName, 
            Token = _tokenService.CreateToken(user)
        };
        return new ApiResponse<UserIdentityDto>(200, userIdentityDto);
    }
    
    [API.Attributes.Authorize]
    [HttpGet]
    public ActionResult<UserIdentityDto> GetCurrentUser()
    {
        var context = HttpContext;
        var user = context.Items["User"] as User;

        return new UserIdentityDto
        {
            Email = user!.Email,
            Token = _tokenService.CreateToken(user)
        };
    }
    
    [HttpGet("emailexists")]
    public async Task<ApiResponse<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
        var userFound = await _userManager.FindByEmailAsync(email) != null;
        return userFound 
            ? new ApiResponse<bool>(200, userFound) 
            : new ApiResponse<bool>(401, "User not found");
    }

    [API.Attributes.Authorize]
    [HttpGet("address")]
    public async Task<ApiResponse<UserAddressDto>> GetUserAddressById(string id)
    {
        var user =  await _userManager.Users.Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Id == id);

        if(user == null)
            return new ApiResponse<UserAddressDto>(401, "User not found");
        
        return new ApiResponse<UserAddressDto>(200,_mapper.Map<UserAddress, UserAddressDto>(user.Address));
    }

    [API.Attributes.Authorize]
    [HttpPut("address")]
    public async Task<ApiResponse<UserAddressDto>> UpdateUserAddress(UserAddressDto address)
    {
        var context = HttpContext;
        var userFromContext = context.Items["User"] as User;
        var user = await _userManager.Users.Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Email == userFromContext.Email);
        
        if(user == null)
            return new ApiResponse<UserAddressDto>(401, "User not found");
        
        user.Address = _mapper.Map<UserAddressDto, UserAddress>(address);

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded 
            ? new ApiResponse<UserAddressDto>(200,_mapper.Map<UserAddressDto>(user.Address)) 
            : new ApiResponse<UserAddressDto>(400, "Problem updating the user");
    }
}
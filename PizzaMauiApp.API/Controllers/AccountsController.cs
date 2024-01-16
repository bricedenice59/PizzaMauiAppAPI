using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Core.Models.Identity;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Errors;

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
    public async Task<ActionResult<UserIdentityDto>> Login(UserLoginDto loginData)
    {
        var user = await _userManager.FindByEmailAsync(loginData.Email);
        if (user == null)
            return Unauthorized(new ApiResponse(401));

        var loginResult = await _signInManager.CheckPasswordSignInAsync(user, loginData.Password, true);
        if(!loginResult.Succeeded)
            return Unauthorized(new ApiResponse(401));

        var userIdentityDto = 
            new UserIdentityDto
            {
                Id = new Guid(user.Id),
                Email = loginData.Email, 
                FirstName = user.DisplayName, 
                Token = _tokenService.CreateToken(user)
            };
        
        return userIdentityDto;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserIdentityDto>> Register(UserRegisterDto registerData)
    {
        if (await _userManager.FindByEmailAsync(registerData.Email) != null)
            return new BadRequestObjectResult(new ApiResponse (400, "Email address is in use"));
        
        var user = new User
        {
            DisplayName = registerData.DisplayName,
            UserName = registerData.DisplayName,
            Email = registerData.Email
        };

        var registerResult = await _userManager.CreateAsync(user, registerData.Password);
        if (!registerResult.Succeeded)
        {
            StringBuilder sb = new();
            foreach (var error in registerResult.Errors)
            {
                sb.AppendLine($"{error.Code}, {error.Description}");
            }
            return BadRequest(new ApiResponse(400, sb.ToString()));
        }
   
        return new UserIdentityDto
        {
            Id = new Guid(user.Id),
            Email = registerData.Email,
            FirstName = user.DisplayName, 
            Token = _tokenService.CreateToken(user)
        };
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
            Token = _tokenService.CreateToken(user),
            FirstName = user!.DisplayName
        };
    }
    
    [HttpGet("emailexists")]
    public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    [API.Attributes.Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<UserAddressDto>> GetUserAddressById(string id)
    {
        //var context = HttpContext;
        //var userFromContext = context.Items["User"] as User;
        
        var user =  await _userManager.Users.Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Id == id);

        if(user == null)
            return NotFound(new ApiResponse(401));
        
        return _mapper.Map<UserAddress, UserAddressDto>(user.Address);
    }

    [API.Attributes.Authorize]
    [HttpPut("address")]
    public async Task<ActionResult<UserAddressDto>> UpdateUserAddress(UserAddressDto address)
    {
        var context = HttpContext;
        var userFromContext = context.Items["User"] as User;
        var user = await _userManager.Users.Include(x => x.Address)
            .SingleOrDefaultAsync(x => x.Email == userFromContext.Email);
        
        if(user == null)
            return NotFound(new ApiResponse(401));
        
        user.Address = _mapper.Map<UserAddressDto, UserAddress>(address);

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded) return Ok(_mapper.Map<UserAddressDto>(user.Address));

        return BadRequest("Problem updating the user");
    }
}
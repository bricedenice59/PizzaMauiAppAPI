using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Core.Models.Identity;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;

namespace PizzaMauiApp.API.Controllers;

public class TokenController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    
    public TokenController(ITokenService tokenService,
        UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }
    
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ApiResponse<UserIdentityDto>> Refresh(TokenModelDto tokenModel)
    {
        string? accessToken = tokenModel.AccessToken;
        string? refreshToken = tokenModel.RefreshToken;

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return new ApiResponse<UserIdentityDto>(400, "Invalid access token or refresh token");
        }
        
        var claim = principal!.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.GivenName);
        if(claim == null)
        {
            return new ApiResponse<UserIdentityDto>(400, "Claimtype givenName has been found in token");
        }
        if(string.IsNullOrEmpty(claim.Value))
        {
            return new ApiResponse<UserIdentityDto>(400, "Claimtype givenName is null or empty");
        } 
        
        var user = await _userManager.FindByNameAsync(claim.Value);
        
        if (user == null)
        {
            return new ApiResponse<UserIdentityDto>(400, "No user found");
        }
        if (user.RefreshToken != refreshToken)
        {
            return new ApiResponse<UserIdentityDto>(400, "Refresh token from database is different than the one passed in parameter");
        }
        if (user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return new ApiResponse<UserIdentityDto>(400, "Refresh token expired");
        }
        
        var newToken = _tokenService.CreateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        var userIdentityDto = 
            new UserIdentityDto
            {
                Id = new Guid(user!.Id),
                Email = user!.Email, 
                FirstName = user.DisplayName, 
                Token = newToken,
                RefreshToken = newRefreshToken
            };
        
        return new ApiResponse<UserIdentityDto>(200, userIdentityDto);
    }
    
    [API.Attributes.Authorize]
    [HttpPost("revoke/{useremail}")]
    public async Task<ApiResponse<string>> Revoke(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return new ApiResponse<string>(401, "User not found.");
        
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return new ApiResponse<string>(204, "No Content");
    }
    
}
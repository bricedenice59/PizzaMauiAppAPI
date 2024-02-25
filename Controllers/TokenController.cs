using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PizzaMauiApp.API.Core.Models;
using PizzaMauiApp.API.Core.Services;
using PizzaMauiApp.API.Dtos;
using PizzaMauiApp.API.Helpers.API;
using PizzaMauiApp.API.Models.Identity;

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
        if (principal is null)
        {
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status400BadRequest, "Invalid access token or refresh token");
        }
        
        var claim = principal!.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.GivenName);
        if(claim is null)
        {
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status400BadRequest, "Claimtype givenName has been found in token");
        }
        if(string.IsNullOrEmpty(claim.Value))
        {
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status400BadRequest, "Claimtype givenName is null or empty");
        } 
        
        var user = await _userManager.FindByNameAsync(claim.Value);
        
        if (user is null)
        {
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status404NotFound, "No user found");
        }
        if (user.RefreshToken != refreshToken)
        {
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status400BadRequest, "Refresh token from database is different than the one passed in parameter");
        }
        if (user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return new ApiResponse<UserIdentityDto>(StatusCodes.Status400BadRequest, "Refresh token expired");
        }
        
        var newToken = _tokenService.CreateToken(new TokenUser { Email = user!.Email, Name = user.DisplayName });
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
        
        return new ApiResponse<UserIdentityDto>(StatusCodes.Status200OK, userIdentityDto);
    }
    
    [API.Attributes.Authorize]
    [HttpPost("revoke/{useremail}")]
    public async Task<ApiResponse<string>> Revoke(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null)
            return new ApiResponse<string>(StatusCodes.Status404NotFound, "User not found.");
        
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return new ApiResponse<string>(StatusCodes.Status204NoContent, "No Content");
    }
    
}
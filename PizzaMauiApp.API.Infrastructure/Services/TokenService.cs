using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Core.Models.Identity;
using PizzaMauiApp.API.Infrastructure.EnvironmentConfig;

namespace PizzaMauiApp.API.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly double _expirationDelay;
    private readonly string _issuer;
    
    public TokenService(TokenAuth0Config tokenConfig)
    {
        _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.Secret!));
        _issuer = tokenConfig.Issuer!;
        _expirationDelay = Convert.ToDouble(tokenConfig.TokenExpirationDelay);
    }
    
    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.DisplayName)
        };
    
        var credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);
    
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.Now.ToUniversalTime(),
            Expires = DateTime.Now.AddSeconds(_expirationDelay).ToUniversalTime(),
            SigningCredentials = credentials,
            Issuer = _issuer
        };
    
        var tokenHandler = new JwtSecurityTokenHandler();
    
        var token = tokenHandler.CreateToken(tokenDescriptor);
    
        return tokenHandler.WriteToken(token);
    }

    public User? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token)) 
            return null;
        
        try
        {
            var jwtToken = GetJwtSecurityToken(token);
            if (jwtToken == null)
                return null;
            
            var userEmail = jwtToken.Claims.First(x => x.Type == "email").Value;
            var displayName = jwtToken.Claims.First(x => x.Type == "given_name").Value;

            // return user id from JWT token if validation successful
            return new User {Email = userEmail, DisplayName = displayName};
        }
        catch
        {
            return null;
        }
    }

    private JwtSecurityToken? GetJwtSecurityToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _symmetricSecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return validatedToken as JwtSecurityToken;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
    
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _symmetricSecurityKey,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            return tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken _);
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaMauiApp.API.Core.Interfaces;
using PizzaMauiApp.API.Core.Models.Identity;

namespace PizzaMauiApp.API.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly double _expirationDelay;
    private readonly string _issuer;
    
    public TokenService(IConfiguration config)
    {
        var auth0Secret = config["Auth0Secret"];
        var auth0Issuer = config["Auth0Issuer"];
        var auth0TokenExpirationDelay = config["Auth0:TokenExpirationDelay"];
        if (string.IsNullOrEmpty(auth0Secret))
            throw new ArgumentNullException("Setting is missing: Auth0:Secret; Add Auth0Secret key in dotnet user-secrets for this project");
        
        if (string.IsNullOrEmpty(auth0Issuer))
            throw new ArgumentNullException("Setting is missing: Auth0:Issuer; Add Auth0Issuer key in dotnet user-secrets for this project");

        if (string.IsNullOrEmpty(auth0TokenExpirationDelay))
            throw new ArgumentNullException("Setting is missing: Auth0:TokenExpirationDelay");
        
        _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(auth0Secret));
        _issuer = auth0Issuer;
        _expirationDelay = Convert.ToDouble(auth0TokenExpirationDelay);
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

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _symmetricSecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            
            var userEmail = jwtToken.Claims.First(x => x.Type == "email").Value;
            var displayName = jwtToken.Claims.First(x => x.Type == "given_name").Value;

            // return user id from JWT token if validation successful
            return new User {Email = userEmail, DisplayName = displayName};
        }
        catch(Exception)
        {
            // return null if validation fails
            return null;
        }
    }
}
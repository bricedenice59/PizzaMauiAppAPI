using PizzaMauiApp.API.Core.Services;

namespace PizzaMauiApp.API.Middlewares;

public class JwtTokensMiddleware
{
    private readonly RequestDelegate _next;

    public JwtTokensMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (!string.IsNullOrEmpty(token))
        {
            var user = tokenService.ValidateToken(token);
            if (user != null)
            {
                // attach user to context on successful jwt validation
                context.Items["User"] = user;
            }
        }
        
        await _next(context);
    }
}
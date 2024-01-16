using System.ComponentModel.DataAnnotations;

namespace PizzaMauiApp.API.Dtos;

public class UserRegisterDto
{
    public required string DisplayName { get; set; }
    
    [EmailAddress]
    public required string Email { get; set; }
    
    //regexlib.com
    [RegularExpression("(?=^.{10,60}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
        ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 10 characters")]
    public required string Password { get; set; }
    
}
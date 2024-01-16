namespace PizzaMauiApp.API.Dtos;

public class UserIdentityDto
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    
}
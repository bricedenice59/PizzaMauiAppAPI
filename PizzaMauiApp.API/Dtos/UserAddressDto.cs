using System.ComponentModel.DataAnnotations;

namespace PizzaMauiApp.API.Dtos;

public class UserAddressDto
{
    public required string Id { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string Street { get; set; }
    
    public required string City { get; set; }

    public required string State { get; set; }
    
    public required string Zipcode { get; set; }
}
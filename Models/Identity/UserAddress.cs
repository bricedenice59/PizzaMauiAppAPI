using System.ComponentModel.DataAnnotations;

namespace PizzaMauiApp.API.Models.Identity;


public class UserAddress
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }

    [Required]
    public string AppUserId { get; set; }
    public User AppUser { get; set; }
}
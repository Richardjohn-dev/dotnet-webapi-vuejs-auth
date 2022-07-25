using Microsoft.AspNetCore.Identity;

namespace AuthIdentity.API.Models;

public class ApiUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsCustomer { get; set; }
}

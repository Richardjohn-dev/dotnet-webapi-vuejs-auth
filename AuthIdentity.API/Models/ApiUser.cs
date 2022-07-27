using Microsoft.AspNetCore.Identity;

namespace RoleBasedAuthIdentity.API.Models;

public class ApiUser : IdentityUser
{
    [PersonalData]
    public string FirstName { get; set; } = string.Empty;
    [PersonalData]
    public string LastName { get; set; } = string.Empty;

    [ProtectedPersonalData]
    public string Gender { get; set; } = string.Empty;
    public bool IsCustomer { get; set; }
}

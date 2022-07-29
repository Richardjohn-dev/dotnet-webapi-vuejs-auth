using Microsoft.AspNetCore.Identity;

namespace RoleBasedIdentityAuthentication.API.Models;

public class User : IdentityUser
{
    // to do 
    // require field lengths all over.
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    public ICollection<Integration> Integrations = new HashSet<Integration>();
}
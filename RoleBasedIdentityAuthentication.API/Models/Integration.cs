namespace RoleBasedIdentityAuthentication.API.Models;

public class Integration
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    //public string UserId { get; set; }
    public User User { get; set; }

}

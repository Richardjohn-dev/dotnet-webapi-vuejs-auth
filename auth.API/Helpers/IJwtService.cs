using System.IdentityModel.Tokens.Jwt;

namespace Auth.API.Helpers
{
    public interface IJwtService
    {
        string Generate(int id);
        JwtSecurityToken Verify(string jwt);
    }
}
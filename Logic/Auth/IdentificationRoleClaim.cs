using Microsoft.AspNetCore.Authorization;

namespace CoTEC_Server.Logic.Auth
{
    public class IdentificationRoleClaim : IAuthorizationRequirement
    {
        public IdentificationRoleClaim(string role)
        {
            Role = role;
        }

        public string Role { get; }
    }

}

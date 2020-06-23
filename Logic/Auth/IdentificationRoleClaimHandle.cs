using CoTEC_Server.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing.Template;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.Auth
{
    public class IdentificationRoleClaimHandle : AuthorizationHandler<IdentificationRoleClaim>
    {
        private readonly SQLServerContext db;

        public IdentificationRoleClaimHandle(SQLServerContext db)
        {
            this.db = db;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IdentificationRoleClaim requirement)
        {

            //TODO: PUt code to verify role of user.
            return Task.CompletedTask;


        }

    }
}

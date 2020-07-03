using CoTEC_Server.DBModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.Auth
{
    public class IdentificationRoleClaimHandle : AuthorizationHandler<IdentificationRoleClaim>
    {
        private readonly CoTEC_DBContext db;

        public IdentificationRoleClaimHandle(CoTEC_DBContext db)
        {
            this.db = db;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IdentificationRoleClaim requirement)
        {
            context.Succeed(requirement);

            //TODO: PUt code to verify role of user.
            return Task.CompletedTask;


        }

    }
}

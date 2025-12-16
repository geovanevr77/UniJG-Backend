using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace UniJG_Backend.Filters
{
    public class GroupsAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string[] groups;

        public GroupsAuthorizationFilter(
            string[] groups)
        {
            this.groups = groups;
        }

        public void OnAuthorization(
            AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
            }

            if (!ReceiveClaims(context, groups))
            {
                context.Result = new ForbidResult();
            }
        }

        internal static bool ReceiveClaims(
            AuthorizationFilterContext context,
            string[] groupsAllowed)
        {
            IEnumerable<Claim> claims = context.HttpContext.User.Claims;
            return IsUserAllowed(claims, groupsAllowed);
        }

        internal static bool IsUserAllowed(
            IEnumerable<Claim> claims,
            string[] groupsAllowed)
        {
            return claims.Any(c => c.Type.Equals(ClaimTypes.Role) && groupsAllowed.Contains(c.Value));
        }
    }
}
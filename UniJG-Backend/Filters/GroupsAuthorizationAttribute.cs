using Microsoft.AspNetCore.Mvc;

namespace UniJG_Backend.Filters
{
    public class GroupsAuthorizationAttribute : TypeFilterAttribute
    {
        public GroupsAuthorizationAttribute(string[] groups) :
            base(typeof(GroupsAuthorizationFilter))
        {
            Arguments = [groups];
        }
    }
}
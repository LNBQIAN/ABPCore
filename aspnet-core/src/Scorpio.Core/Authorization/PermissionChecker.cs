using Abp.Authorization;
using Scorpio.Authorization.Roles;
using Scorpio.Authorization.Users;

namespace Scorpio.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}

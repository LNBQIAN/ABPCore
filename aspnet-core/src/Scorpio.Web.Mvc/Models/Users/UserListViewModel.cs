using System.Collections.Generic;
using Scorpio.Roles.Dto;
using Scorpio.Users.Dto;

namespace Scorpio.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}

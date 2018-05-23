using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Scorpio.Roles.Dto;
using Scorpio.Users.Dto;

namespace Scorpio.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}

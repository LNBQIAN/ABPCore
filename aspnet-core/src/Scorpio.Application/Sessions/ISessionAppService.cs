using System.Threading.Tasks;
using Abp.Application.Services;
using Scorpio.Sessions.Dto;

namespace Scorpio.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}

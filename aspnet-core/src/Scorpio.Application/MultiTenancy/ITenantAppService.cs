using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Scorpio.MultiTenancy.Dto;

namespace Scorpio.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

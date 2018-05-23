﻿using System.Threading.Tasks;
using Abp.Application.Services;
using Scorpio.Authorization.Accounts.Dto;

namespace Scorpio.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}

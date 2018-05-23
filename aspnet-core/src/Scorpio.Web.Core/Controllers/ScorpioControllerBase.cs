using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Scorpio.Controllers
{
    public abstract class ScorpioControllerBase: AbpController
    {
        protected ScorpioControllerBase()
        {
            LocalizationSourceName = ScorpioConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}

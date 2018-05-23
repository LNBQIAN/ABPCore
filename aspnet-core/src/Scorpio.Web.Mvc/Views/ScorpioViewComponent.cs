using Abp.AspNetCore.Mvc.ViewComponents;

namespace Scorpio.Web.Views
{
    public abstract class ScorpioViewComponent : AbpViewComponent
    {
        protected ScorpioViewComponent()
        {
            LocalizationSourceName = ScorpioConsts.LocalizationSourceName;
        }
    }
}

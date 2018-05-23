using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;

namespace Scorpio.Web.Views
{
    public abstract class ScorpioRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected ScorpioRazorPage()
        {
            LocalizationSourceName = ScorpioConsts.LocalizationSourceName;
        }
    }
}

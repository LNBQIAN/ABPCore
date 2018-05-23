using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using Scorpio.Controllers;

namespace Scorpio.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : ScorpioControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}

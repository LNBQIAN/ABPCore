using Microsoft.AspNetCore.Antiforgery;
using Scorpio.Controllers;

namespace Scorpio.Web.Host.Controllers
{
    public class AntiForgeryController : ScorpioControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}

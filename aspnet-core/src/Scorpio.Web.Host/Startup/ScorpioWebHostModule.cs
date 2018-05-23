using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Scorpio.Configuration;

namespace Scorpio.Web.Host.Startup
{
    [DependsOn(
       typeof(ScorpioWebCoreModule))]
    public class ScorpioWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ScorpioWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ScorpioWebHostModule).GetAssembly());
        }
    }
}

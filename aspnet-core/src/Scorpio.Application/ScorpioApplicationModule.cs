using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Scorpio.Authorization;

namespace Scorpio
{
    [DependsOn(
        typeof(ScorpioCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ScorpioApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ScorpioAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(ScorpioApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}

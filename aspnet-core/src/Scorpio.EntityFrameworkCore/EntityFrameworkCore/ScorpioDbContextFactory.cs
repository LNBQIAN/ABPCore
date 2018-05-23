using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Scorpio.Configuration;
using Scorpio.Web;

namespace Scorpio.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ScorpioDbContextFactory : IDesignTimeDbContextFactory<ScorpioDbContext>
    {
        public ScorpioDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ScorpioDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            ScorpioDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ScorpioConsts.ConnectionStringName));

            return new ScorpioDbContext(builder.Options);
        }
    }
}

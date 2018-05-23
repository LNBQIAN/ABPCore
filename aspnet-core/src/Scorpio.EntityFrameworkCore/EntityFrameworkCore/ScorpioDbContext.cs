using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Scorpio.Authorization.Roles;
using Scorpio.Authorization.Users;
using Scorpio.MultiTenancy;

namespace Scorpio.EntityFrameworkCore
{
    public class ScorpioDbContext : AbpZeroDbContext<Tenant, Role, User, ScorpioDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public ScorpioDbContext(DbContextOptions<ScorpioDbContext> options)
            : base(options)
        {
        }
    }
}

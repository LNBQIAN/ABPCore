using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Scorpio.EntityFrameworkCore
{
    public static class ScorpioDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ScorpioDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ScorpioDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}

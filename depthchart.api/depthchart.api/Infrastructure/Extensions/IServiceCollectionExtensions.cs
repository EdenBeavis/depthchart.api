using depthchart.api.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;

namespace depthchart.api.Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void SetupDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = new SqliteConnection(configuration.GetConnectionString("DepthChartDb"));

            services.AddEntityFrameworkSqlite();
            services.AddDbContext<DepthChartContext>(options => options.UseSqlite(connection));
        }

        public static void SetupLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddNLog();
            });
        }
    }
}
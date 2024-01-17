using depthchart.api.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using System.Data.Common;

namespace depthchart.api.Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void SetupDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DbConnection>(container =>
            {
                var connectionString = configuration.GetConnectionString("DepthChartDb");
                var connection = new SqliteConnection(connectionString);
                connection.Open();

                return connection;
            });

            services.AddDbContext<DepthChartContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
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
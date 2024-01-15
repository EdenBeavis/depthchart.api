using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace depthchart.api.Infrastructure.Extensions
{
    public class ServiceCollectionExtensions
    {
        protected virtual void SetupDatabases(IServiceCollection services)
        {
            var connection = new SqliteConnection(Configuration.GetConnectionString("ProductsDb"));

            connection.Open();
            services.AddEntityFrameworkSqlite();
            services.AddDbContext<ProductsDbContext>(options => options.UseSqlite(connection));
        }

        protected void SetupLogging(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddNLog();
            });
        }
    }
}
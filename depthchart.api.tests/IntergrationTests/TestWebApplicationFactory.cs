using depthchart.api.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using System.Data.Common;

namespace depthchart.api.tests.IntergrationTests
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .ConfigureTestServices((services) =>
                {
                    services.AddSingleton<DbConnection>(container =>
                    {
                        var connection = new SqliteConnection("DataSource=:memory:");
                        connection.Open();

                        return connection;
                    });

                    services.AddDbContext<DepthChartContext>((container, options) =>
                    {
                        var connection = container.GetRequiredService<DbConnection>();
                        options.UseSqlite(connection);
                    });
                })
                .ConfigureServices((services) =>
                {
                    services.AddScoped<RefDataProvider>();
                })
                .ConfigureAppConfiguration((host, configurationBuilder) => { })
                .UseNLog()
                .UseEnvironment(Environments.Development);
        }

        public HttpClient Client => CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
}
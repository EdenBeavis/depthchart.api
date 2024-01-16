using depthchart.api.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;

namespace depthchart.api.tests.IntergrationTests
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .ConfigureServices((services) =>
                {
                    var connection = new SqliteConnection("Datasource=:memory:");
                    connection.Open();
                    services.AddEntityFrameworkSqlite();
                    services.AddDbContext<DepthChartContext>(options => options.UseSqlite(connection));
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
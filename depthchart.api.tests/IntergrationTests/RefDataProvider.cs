using depthchart.api.Infrastructure.Data;
using depthchart.api.Infrastructure.Data.Entities;
using depthchart.api.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.tests.IntergrationTests
{
    public class RefDataProvider
    {
        private readonly DepthChartContext _depthChartContext;

        public RefDataProvider(DepthChartContext productsDbContext)
        {
            _depthChartContext = productsDbContext;
        }

        public void SetupRefData()
        {
            DeleteRefData();

            _depthChartContext.Database.GetDbConnection().Open();
            _depthChartContext.Database.EnsureCreated();
            _depthChartContext.SaveChanges();

            _depthChartContext.Players.Add(new Player
            {
                Id = 1,
                Name = "Test 1",
                Number = 1,
                Sport = SportType.NFL
            });
            _depthChartContext.Players.Add(new Player
            {
                Id = 2,
                Name = "Test 2",
                Number = 2,
                Sport = SportType.NFL
            });
            _depthChartContext.Players.Add(new Player
            {
                Id = 3,
                Name = "Test 3",
                Number = 3,
                Sport = SportType.NFL
            });

            _depthChartContext.SaveChanges();
        }

        private void DeleteRefData()
        {
            _depthChartContext.ChangeTracker.Entries().ToList().All(x =>
            {
                x.State = EntityState.Detached;
                return true;
            });

            _depthChartContext.Database.GetDbConnection().Close();
            _depthChartContext.Database.EnsureDeleted();
        }
    }
}
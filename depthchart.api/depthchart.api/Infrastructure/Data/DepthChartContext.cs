using depthchart.api.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Infrastructure.Data
{
    public class DepthChartContext : DbContext
    {
        public DepthChartContext(DbContextOptions<DepthChartContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var type = typeof(IEntityConfiguration);

            typeof(DepthChartContext).Assembly
                .GetTypes()
                .Where(t => type.IsAssignableFrom(t) && t.IsClass)
                .Select(Activator.CreateInstance)
                .Cast<IEntityConfiguration>()
                .All(configuration =>
                {
                    configuration.Configure(modelBuilder);
                    return true;
                });
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<DepthPosition> DepthPositions { get; set; }
    }
}
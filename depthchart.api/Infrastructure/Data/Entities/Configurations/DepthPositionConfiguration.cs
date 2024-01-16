using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Infrastructure.Data.Entities.Configurations
{
    public class DepthPositionConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<DepthPosition>();

            entity.ToTable("DepthPositions");

            entity.HasOne(dp => dp.Player)
                .WithMany(p => p.DepthPositions)
                .HasForeignKey(dp => dp.PlayerId);
        }
    }
}
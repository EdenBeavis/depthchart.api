using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Infrastructure.Data.Entities.Configurations
{
    public class PlayerConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Player>();

            entity.ToTable("Players");

            entity.HasMany(p => p.DepthPositions)
                .WithOne(dp => dp.Player)
                .HasForeignKey(dp => dp.PlayerId);
        }
    }
}
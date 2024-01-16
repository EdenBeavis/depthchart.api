using Microsoft.EntityFrameworkCore;

namespace depthchart.api.Infrastructure.Data
{
    public interface IEntityConfiguration
    {
        void Configure(ModelBuilder modelBuilder);
    }
}
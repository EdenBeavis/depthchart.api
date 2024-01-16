using depthchart.api.Models;

namespace depthchart.api.Infrastructure.Data.Entities
{
    public class DepthChartRow
    {
        public string Position { get; init; } = string.Empty;
        public IEnumerable<Player> Players { get; init; } = Enumerable.Empty<Player>();
    }
}
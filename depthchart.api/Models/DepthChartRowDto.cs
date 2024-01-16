namespace depthchart.api.Models
{
    public class DepthChartRowDto
    {
        public string Position { get; init; } = string.Empty;
        public IEnumerable<PlayerDto> Players { get; init; } = Enumerable.Empty<PlayerDto>();
    }
}
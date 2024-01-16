namespace depthchart.api.Models
{
    public class DepthPositionDto
    {
        public PlayerDto Player { get; set; }

        public string PlayerPosition { get; set; } = string.Empty;
        public int Depth { get; set; } = default;
    }
}
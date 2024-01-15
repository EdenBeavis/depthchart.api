namespace depthchart.api.Infrastructure.Data.Entities
{
    public class DepthPosition : EntityBase
    {
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public string PlayerPosition { get; set; } = string.Empty;
        public int Depth { get; set; } = default;

        public DepthPosition()
        {
        }

        public DepthPosition(string playerPosition, Player player, int depth)
        {
            Player = player;
            PlayerPosition = playerPosition;
            Depth = depth;
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace depthchart.api.Infrastructure.Data.Entities
{
    public class DepthPosition : EntityBase
    {
        public int PlayerId { get; set; }

        [Required]
        public Player Player { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1, ErrorMessage = Constants.ErrorMessages.DepthPostionPosition)]
        public string PlayerPosition { get; set; } = string.Empty;

        [Required]
        [Range(0, 99, ErrorMessage = Constants.ErrorMessages.DepthPostionDepth)]
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
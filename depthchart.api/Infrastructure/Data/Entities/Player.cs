using depthchart.api.Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace depthchart.api.Infrastructure.Data.Entities
{
    public class Player : EntityBase
    {
        // Thoughts for scaling
        // Numbers in NBA can be 00 so this data type would need to be string
        // to work with the NBA.
        [Required]
        [Range(0, 99, ErrorMessage = Constants.ErrorMessages.PlayerNumber)]
        public int Number { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1, ErrorMessage = Constants.ErrorMessages.PlayerName)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<DepthPosition> DepthPositions { get; set; } = new List<DepthPosition>();

        public SportType Sport { get; set; } = SportType.None;
    }
}
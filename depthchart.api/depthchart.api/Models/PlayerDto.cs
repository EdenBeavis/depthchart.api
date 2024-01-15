using depthchart.api.Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace depthchart.api.Models
{
    public class PlayerDto
    {
        public int Id { get; set; }

        // Thoughts for scaling
        // Numbers in NBA can be 00 so this data type would need to be string
        // to work with the NBA.

        [Range(0, 99, ErrorMessage = "Player number must be equal to or greater than 0 and equal to or less than 100.")]
        public int Number { get; set; }

        [StringLength(255, MinimumLength = 1, ErrorMessage = "Player name must have a value and be less than 256 characters.")]
        public string Name { get; set; } = string.Empty;

        public SportType Sport { get; set; } = SportType.None;
    }
}
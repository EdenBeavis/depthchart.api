using depthchart.api.Infrastructure.Enums;

namespace depthchart.api.Infrastructure.Data.Entities
{
    public class Player : EntityBase
    {
        // Thoughts for scaling
        // Numbers in NBA can be 00 so this data type would need to be string
        // to work with the NBA.
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;

        public SportType Sport { get; set; } = SportType.None;
    }
}
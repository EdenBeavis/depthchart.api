using System.ComponentModel.DataAnnotations.Schema;

namespace depthchart.api.Infrastructure.Data.Entities
{
    public abstract class EntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
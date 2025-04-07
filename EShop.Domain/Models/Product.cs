using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Domain.Models
{
    public class Product : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = default!;

        [Required]
        [MaxLength(13)]
        public string Ean { get; set; } = default!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; } = default!;

        public int Stock { get; set; }

        [Required]
        [MaxLength(50)]
        public string Sku { get; set; } = default!;

        [Required]
        public required Category Category { get; set; }
    }
}

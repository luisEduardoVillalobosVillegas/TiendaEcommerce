using System.ComponentModel.DataAnnotations;

namespace MyStore.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageName { get; set; } = null;

        public int Stock { get; set; }

        public Category? Category { get; set; }
    }
}

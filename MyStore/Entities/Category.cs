using System.ComponentModel.DataAnnotations;

namespace MyStore.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}

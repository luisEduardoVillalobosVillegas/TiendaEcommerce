using Microsoft.AspNetCore.Mvc.Rendering;
using MyStore.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Models
{
    public class ProductVM
    {
        //pegamos todas las propiedades de la entidad product y modificamos en base al VM
        public int ProductId { get; set; }

        public CategoryVM Category { get; set; }

        //se utilizara para combo box
        public List<SelectListItem> Categories { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }

        public string? ImageName { get; set; } = null;
        [Required]
        public int Stock { get; set; }
        //este se modifica para agregar imagenes
        public IFormFile? ImageFile { get; set; }
    }
}

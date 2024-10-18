using ProductMicroService.Data;
using System.ComponentModel.DataAnnotations;

namespace ProductMicroService.Data.DTOs
{
    public class NewProductDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string PictureURL { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public ProductCategory ProductCategory { get; set; }

        [Required]
        public bool AvailableForSale { get; set; } = true;
    }
}

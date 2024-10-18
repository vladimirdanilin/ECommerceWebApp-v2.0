using System.ComponentModel.DataAnnotations;

namespace ProductMicroService.Data.Models
{
    public class Product
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

        public int TotalNumberOfStars { get; set; } = 0;

        public int TotalNumberOfRates { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public ProductCategory ProductCategory { get; set; }

        [Required]
        public bool AvailableForSale { get; set; } = true;
    }
}

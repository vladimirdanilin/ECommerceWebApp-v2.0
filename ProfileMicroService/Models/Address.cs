using System.ComponentModel.DataAnnotations;

namespace ProfileMicroService.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        public int UserId { get; set; }

        public bool IsDefaultAddress { get; set; } = false;
    }
}

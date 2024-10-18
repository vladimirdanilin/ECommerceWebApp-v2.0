using Microsoft.AspNetCore.Identity;

namespace AuthMicroService.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? ProfilePictureURL { get; set; }
    }
}

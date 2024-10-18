﻿namespace AuthMicroService.Data.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? ProfilePictureURL { get; set; }
    }
}

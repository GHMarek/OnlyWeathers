﻿namespace OnlyWeathersApi.Models.DTO
{
    public class RegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

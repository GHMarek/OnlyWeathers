﻿namespace OnlyWeathersApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }

        public List<FavoriteCity> FavoriteCities { get; set; } = new();

    }
}

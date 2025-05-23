﻿using OnlyWeathersApi.Models;
using static OnlyWeathersAPI.Services.UserService;

namespace OnlyWeathersApi.Services
{
    public interface IUserService
    {
        Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
        Task<RegisterResult> RegisterAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);

    }
}

﻿namespace BookStoreApp.API.Models.User
{
    public class AuthResponse
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}

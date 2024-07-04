﻿namespace TasklistAPI.Model.Entity
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
    }
}

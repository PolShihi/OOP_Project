﻿using Microsoft.AspNetCore.Identity;

namespace Server_side.Data
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

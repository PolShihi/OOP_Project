using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModel.Models.DTOs
{
    public class UserSession
    {
        public string? Id {  get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public string FullName { get => FirstName +  " " + LastName; }

        public string FullNameEmail { get => $"{FirstName} {LastName} ({Email})"; }
    }
}

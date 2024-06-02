using MyModel.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModel.Models.Entitties
{
    public class Client : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool IsProcessed { get; set; }

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        public Order? Order { get; set; }

        public UserSession? UserSession { get; set; }

        public string UserInfo
        {
            get => UserSession is null ? "no user" : UserSession.FullNameEmail;
        }

        public string FullName { get => FirstName + " " + LastName; }
    }
}

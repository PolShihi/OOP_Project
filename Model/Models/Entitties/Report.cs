using MyModel.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModel.Models.Entitties
{
    public class Report : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Text { get; set; }
        public bool IsAnswered { get; set; }
        public string Answer { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        public UserSession? UserSession { get; set; }

        public string UserInfo
        {
            get => UserSession is null ? "no user" : UserSession.FullNameEmail;
        }

        public string FullName { get => FirstName + " " + LastName; }

        public bool IsNotAnswered { get => !IsAnswered;}
    }
}

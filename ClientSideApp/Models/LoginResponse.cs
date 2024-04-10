using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Models
{
    public class LoginResponse
    {
        public bool Flag { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}

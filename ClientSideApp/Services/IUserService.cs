using ClientSideApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Services
{
    public interface IUserService
    {
        Task<LoginResponse> Authenticate(LoginRequest loginRequest);

        Task<List<UserInfo>> GetUsers();

        Task Register(RegistrationRequest registrationRequest);

        Task DeleteUser(string id);
    }
}

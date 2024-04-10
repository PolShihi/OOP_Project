using Server_side.Models.DTOs;
using static Server_side.Models.DTOs.ServiceResponces;

namespace Server_side.Repositories
{
    public interface IUserRepository
    {
        Task<GeneralResponse> CreateAccount(UserDTO userDTO);
        Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
        Task<List<UserSession>> GetUsers();
        Task<GeneralResponse> DeleteUser(string id);
    }
}

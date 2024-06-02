using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Services
{
    public interface IUserRepository
    {
        Task<ApiResponse<string>> CreateAccount(RegisterDTO registerDTO);

        Task<ApiResponse<LoginResponseDTO>> LoginAccount(LoginDTO loginDTO);

        Task<ApiResponse<UserSession?>> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<List<UserSession>>> ListAllAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<List<UserSession>>> ListAsync(Func<UserSession, bool> filter, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> UpdateAsync(string id, RegisterDTO user, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> DeleteAsync(string id, CancellationToken cancellationToken = default);

        Task<ApiResponse<UserSession?>> FirstOrDefaultAsync(Func<UserSession, bool> filter, CancellationToken cancellationToken = default);
    }
}

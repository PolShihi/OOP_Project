using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using System.Linq.Expressions;

namespace Server_side.Repositories
{
    public interface IUserRepository
    {
        Task<ApiResponse<string>> CreateAccount(RegisterDTO registerDTO);

        Task<ApiResponse<LoginResponseDTO>> LoginAccount(LoginDTO loginDTO);

        Task<UserSession?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserSession>> ListAllAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserSession>> ListAsync(Expression<Func<AppUser, bool>> filter,
            CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> UpdateAsync(string id, RegisterDTO user, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> DeleteAsync(string id, CancellationToken cancellationToken = default);

        Task<UserSession?> FirstOrDefaultAsync(Expression<Func<AppUser, bool>> filter,
            CancellationToken cancellationToken = default);
    }
}

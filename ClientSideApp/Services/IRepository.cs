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
    public interface IRepository<EntityType, DTO> 
        where EntityType : Entity 
        where DTO : class
    {
        Task<ApiResponse<EntityType?>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ApiResponse<EntityType?>> FirstOrDefaultAsync(Func<EntityType, bool> filter, CancellationToken cancellationToken = default);

        Task<ApiResponse<List<EntityType>>> ListAllAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<List<EntityType>>> ListAsync(Func<EntityType, bool> filter, CancellationToken cancellationToken = default);

        Task<ApiResponse<EntityType?>> AddAsync(DTO dto, CancellationToken cancellationToken = default);

        Task<ApiResponse<EntityType?>> UpdateAsync(int id, DTO dto, CancellationToken cancellationToken = default);

        Task<ApiResponse<string>> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}

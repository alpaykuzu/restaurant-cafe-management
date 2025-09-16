using Core.Dtos.RoleDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRoleService
    {
        Task<ApiResponse<IEnumerable<RoleResponseDto>>> GetAllRolesByRestaurantIdAsync();
        Task<ApiResponse<RoleResponseDto>> GetRoleByIdAsync(int id);
        Task<ApiResponse<IEnumerable<RoleResponseDto>>> GetAllRoleAsync();
        Task<ApiResponse<RoleResponseDto>> CreateRoleAsync(CreateRoleRequestDto req);
        Task<ApiResponse<RoleResponseDto>> UpdateRoleAsync(UpdateRoleRequestDto req);
        Task<ApiResponse<NoContent>> DeleteRoleAsync(DeleteRoleRequestDto req);
    }
}

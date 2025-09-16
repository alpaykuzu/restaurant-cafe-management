using Core.Dtos.AuthDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<UserResponseDto>>> GetAllUserAsync();
        Task<ApiResponse<UserResponseDto>> GetUserByIdAsync();
        Task<ApiResponse<IEnumerable<UserResponseDto>>> GetManagersAsync();
    }
}

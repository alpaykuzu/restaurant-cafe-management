using Core.Dtos.AuthDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<UserResponseDto>> RegisterAsync(RegisterRequestDto registerRequest);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ApiResponse<TokensResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto tokenRequest);
        Task<ApiResponse<UserResponseDto>> MeAsync();
    }
}

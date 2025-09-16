using Core.Dtos.RestaurantDtos;
using Core.Models;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRestaurantService
    {
        Task<ApiResponse<RestaurantResponseDto>> GetRestaurantByUserIdAsync();
        Task<ApiResponse<IEnumerable<RestaurantResponseDto>>> GetAllRestaurantsAsync();
        Task<ApiResponse<NoContent>> CreateRestaurantAsync(CreateRestaurantRequestDto req);
        Task<ApiResponse<NoContent>> UpdateRestaurantAsync(UpdateRestaurantRequestDto req);
        Task<ApiResponse<NoContent>> DeleteRestaurantAsync(int id);
    }
}

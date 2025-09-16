using Core.Dtos.CategoryDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllCategoryByRestaurantIdAsync();
        Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId);
        Task<ApiResponse<NoContent>> CreateCategoryAsync(CreateCategoryRequestDto req);
        Task<ApiResponse<NoContent>> UpdateCategoryAsync(UpdateCategoryRequestDto req);
        Task<ApiResponse<NoContent>> DeleteCategoryAsync(int categoryId);
    }
}

using Core.Dtos.MenuItemDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMenuItemService
    {
        Task<ApiResponse<IEnumerable<MenuItemResponsDto>>> GetAllMenuItemsByRestaurantIdAsync();
        Task<ApiResponse<IEnumerable<MenuItemResponsDto>>> GetMenuItemsByCategoryIdAsync(int categoryId);
        Task<ApiResponse<MenuItemResponsDto>> GetMenuItemByIdAsync(int id);
        Task<ApiResponse<NoContent>> CreateMenuItemAsync(CreateMenuItemRequestDto req);
        Task<ApiResponse<NoContent>> UpdateMenuItemAsync(UpdateMenuItemRequestDto req);
        Task<ApiResponse<NoContent>> UpdatePriceAsync(int id, int newPrice);
        Task<ApiResponse<NoContent>> DeleteMenuItemAsync(int id);
    }
}

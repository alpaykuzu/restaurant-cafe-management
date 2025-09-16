using Core.Dtos.InventoryItemDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IInventoryItemService
    {
        Task<ApiResponse<IEnumerable<InventoryItemResponseDto>>> GetAllInventoryByRestaurantIdAsync(int restaurantId);
        Task<ApiResponse<InventoryItemResponseDto>> GetInventoryItemByIdAsync(int id);
        Task<ApiResponse<InventoryItemResponseDto>> UpdateInventoryItemAsync(UpdateInventoryItemRequestDto req);
        Task<ApiResponse<NoContent>> UpdateStockLevelAsync(int id, int newStockLevel);
        Task<ApiResponse<InventoryItemResponseDto>> CreateInventoryItemAsync(CreateInventoryItemRequestDto req);
        Task<ApiResponse<NoContent>> DeleteInventoryItemAsync(int id);
    }
}

using Core.Dtos.InventoryTransactionDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IInventoryTransactionService
    {
        Task<ApiResponse<IEnumerable<InventoryItemTransactionResponseDto>>> GetAllInventoryTransactionByRestaurantIdAsync(int restaurantId);
        Task<ApiResponse<IEnumerable<InventoryItemTransactionResponseDto>>> GetInventoryTransactionByEmployeeAsync(int employeeId);
        Task<ApiResponse<NoContent>> CreateInventoryTransactionAsync(CreateInventoryTransactionRequestDto req);
        Task<ApiResponse<NoContent>> DeleteInventoryTransactionAsync(int id);
    }
}

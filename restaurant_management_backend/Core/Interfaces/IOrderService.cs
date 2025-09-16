using Core.Dtos.OrderDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<NoContent>> CreateOrderAsync(CreateOrderFullRequestDto req);
        Task<ApiResponse<NoContent>> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetOrdersByRestaurantIdAsync();
        Task<ApiResponse<OrderResponseDto>> GetOrderByIdAsync(int orderId);
        Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetOrdersByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<OrderResponseDto>>> GetOrdersByDateAsync(DateTime startDate);

    }
}

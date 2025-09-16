using Core.Dtos.ReservationDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReservationService
    {
        Task<ApiResponse<ReservationResponseDto>> CreateReservationAsync(CreateReservationRequestDto req);
        Task<ApiResponse<ReservationResponseDto>> UpdateReservationAsync(UpdateReservationRequestDto req);
        Task<ApiResponse<NoContent>> CancelReservationAsync(int id);
        Task<ApiResponse<IEnumerable<ReservationResponseDto>>> GetReservationsByRestaurantIdAsync(int restaurantId);
        Task<ApiResponse<IEnumerable<ReservationResponseDto>>> GetReservationsByCustomerAsync(string customerName);
    }
}

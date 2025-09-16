using Core.Dtos.PaymentDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponseDto>> MakePaymentAsync(CreatePaymentRequestDto req);
        Task<ApiResponse<PaymentResponseDto>> GetPaymentByIdAsync(int paymentId);
        Task<ApiResponse<PaymentResponseDto>> GetPaymentByOrderIdAsync(int orderId);
        Task<ApiResponse<IEnumerable<PaymentResponseDto>>> GetPaymentsByRestaurantIdAsync();
    }
}

using Core.Dtos.InvoiceDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IInvoiceService
    {
        Task<ApiResponse<InvoiceResponseDto>> CreateInvoiceAsync(int orderId);
        Task<ApiResponse<InvoiceResponseDto>> GetInvoiceByIdAsync(int invoiceId);
        Task<ApiResponse<IEnumerable<InvoiceResponseDto>>> GetInvoicesByRestaurantIdAsync();
        Task<ApiResponse<IEnumerable<InvoiceResponseDto>>> GetInvoicesByDateAsync(DateTime startDate);
    }
}

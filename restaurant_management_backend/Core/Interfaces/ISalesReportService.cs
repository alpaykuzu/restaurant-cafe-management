using Core.Dtos.SalesReportDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISalesReportService
    {
        Task<ApiResponse<SalesReportResponseDto>> GenerateReportAsync(CreateSalesReportRequestDto request);
    }
}

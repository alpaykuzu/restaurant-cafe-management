using Core.Dtos.TableDtos;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITableService
    {
        Task<ApiResponse<TableResponseDto>> GetTableByIdAsync(int id);
        Task<ApiResponse<IEnumerable<TableResponseDto>>> GetAllTablesByRestaurantIdAsync();
        Task<ApiResponse<string>> GetTableCountByRestaurantIdAsync();
        Task<ApiResponse<NoContent>> CreateTableAsync(CreateTableRequestDto req);
        Task<ApiResponse<NoContent>> UpdateTableAsync(UpdateTableRequestDto req);
        Task<ApiResponse<NoContent>> DeleteTableAsync(int id);
        Task<ApiResponse<NoContent>> UpdateTableStatusByRestaurantIdAndIdAsync(UpdateTableStatusRequestDto req);
        Task<ApiResponse<string>> GetTableCountByRestaurantIdAndStatusAsync(string status);
        Task<ApiResponse<IEnumerable<TableResponseDto>>> GetTablesByRestaurantIdAndStatusAsync(string status);
    }
}

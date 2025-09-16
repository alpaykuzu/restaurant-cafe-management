using Core.Dtos.EmployeeDtos;
using Core.Models;
using Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmployeeService
    {
        Task<ApiResponse<NoContent>> CreateEmployeeAsync(CreateEmployeeRequestDto req);
        Task<ApiResponse<NoContent>> UpdateEmployeeAsync(UpdateEmployeeRequestDto req);
        Task<ApiResponse<NoContent>> DeleteEmployeeAsync(int id);
        Task<ApiResponse<EmployeeResponseDto>> GetEmployeeByIdAsync(int id);
        Task<ApiResponse<IEnumerable<EmployeeResponseDto>>> GetAllEmployeesAsync();
        Task<ApiResponse<IEnumerable<EmployeeResponseDto>>> GetEmployeesByRestaurantId(int restaurantId);
        Task<ApiResponse<IEnumerable<EmployeeResponseDto>>> GetEmployeesOwnRestaurant();
    }
}

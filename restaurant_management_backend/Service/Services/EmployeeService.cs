using AutoMapper;
using Core.Dtos.CategoryDtos;
using Core.Dtos.EmployeeDtos;
using Core.Extensions;
using Core.Hubs;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IHubContext<RestaurantHub> _hubContext;
        public EmployeeService(IGenericRepository<Employee> employeeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IGenericRepository<User> userRepository, IHubContext<RestaurantHub> hubContext)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<NoContent>> CreateEmployeeAsync(CreateEmployeeRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            }
            if(user.Employee != null)
            {
                if (req.RestaurantId != user.Employee.RestaurantId)
                {
                    return ApiResponse<NoContent>.Fail("Yetkisiz işlem.");
                }
            }
            var employee = await _employeeRepository.GetFirstOrDefaultAsync(e => e.UserId == req.UserId);
            if (employee != null)
            {
                return ApiResponse<NoContent>.Fail("Bu kullanıcı zaten bir çalışan olarak atanmış.");
            }
            var newEmployee = _mapper.Map<Employee>(req);
            await _employeeRepository.AddAsync(newEmployee);
            await _hubContext.Clients.Group(req.RestaurantId.ToString()).SendAsync("EmployeeChanged");
            return ApiResponse<NoContent>.Ok("Çalışan oluşturuldu.");
        }
        public async Task<ApiResponse<NoContent>> DeleteEmployeeAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            }
            var employee = await _employeeRepository.GetByIdAsync(id);
            if(employee.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<NoContent>.Fail("Yetkisiz işlem.");
            }
            if (employee == null)
            {
                return ApiResponse<NoContent>.Fail("Çalışan bulunamadı.");
            }
            employee.IsActive = false;
            await _employeeRepository.UpdateAsync(employee);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("EmployeeChanged");
            return ApiResponse<NoContent>.Ok("Çalışan silindi.");
        }
        public async Task<ApiResponse<IEnumerable<EmployeeResponseDto>>> GetAllEmployeesAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<IEnumerable<EmployeeResponseDto>>.Fail("Kullanıcı bulunamadı.");
            }
            var employees = await _employeeRepository.FindAsync(e => e.IsActive == true);
            if(user.Employee.RestaurantId != employees.First().RestaurantId)
            {
                return ApiResponse<IEnumerable<EmployeeResponseDto>>.Fail("Yetkisiz işlem.");
            }
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
            return ApiResponse<IEnumerable<EmployeeResponseDto>>.Ok(employeeDtos);
        }
        public async Task<ApiResponse<EmployeeResponseDto>> GetEmployeeByIdAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<EmployeeResponseDto>.Fail("Kullanıcı bulunamadı.");
            }
            var employee = await _employeeRepository.Query().Where(e => e.IsActive == true && e.Id == id).FirstOrDefaultAsync();
            if (employee == null)
            {
                return ApiResponse<EmployeeResponseDto>.Fail("Çalışan bulunamadı.");
            }
            if(employee.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<EmployeeResponseDto>.Fail("Yetkisiz işlem.");
            }
            var employeeDto = _mapper.Map<EmployeeResponseDto>(employee);
            return ApiResponse<EmployeeResponseDto>.Ok(employeeDto, "Başarılı.");
        }
        public async Task<ApiResponse<NoContent>> UpdateEmployeeAsync(UpdateEmployeeRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            }
            var employee = await _employeeRepository.GetByIdAsync(req.Id);
            if (employee == null)
            {
                return ApiResponse<NoContent>.Fail("Çalışan bulunamadı.");
            }
            if (employee.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<NoContent>.Fail("Yetkisiz işlem.");
            }
            _mapper.Map(req, employee);
            await _employeeRepository.UpdateAsync(employee);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("EmployeeChanged");
            return ApiResponse<NoContent>.Ok("Çalışan güncellendi.");
        }
        public async Task<ApiResponse<IEnumerable<EmployeeResponseDto>>> GetEmployeesByRestaurantId(int restaurantId)
        {
            var employees = await _employeeRepository.FindAsync(e => e.IsActive == true && e.RestaurantId == restaurantId);
            if (employees == null)
            {
                return ApiResponse<IEnumerable<EmployeeResponseDto>>.Fail("Çalışan bulunamadı.");
            }
            var employeesDtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
            return ApiResponse<IEnumerable<EmployeeResponseDto>>.Ok(employeesDtos, "Başarılı");
        }
        public async Task<ApiResponse<IEnumerable<EmployeeResponseDto>>> GetEmployeesOwnRestaurant()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<IEnumerable<EmployeeResponseDto>>.Fail("Kullanıcı bulunamadı.");
            }
            var employees = await _employeeRepository.FindAsync(e => e.RestaurantId == user.Employee.RestaurantId && e.IsActive == true);
            if (employees == null)
            {
                return ApiResponse<IEnumerable<EmployeeResponseDto>>.Fail("Çalışan bulunamadı.");
            }
            var employeesDtos = _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
            return ApiResponse<IEnumerable<EmployeeResponseDto>>.Ok(employeesDtos, "Başarılı");
        }
    }
}

using AutoMapper;
using Core.Dtos.SalesReportDtos;
using Core.Dtos.TableDtos;
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
    public class TableService : ITableService
    {
        private readonly IGenericRepository<Table> _tableRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IHubContext<RestaurantHub> _hubContext;
        public TableService(IGenericRepository<Table> tableRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IGenericRepository<User> userRepository, IHubContext<RestaurantHub> hubContext)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }
        public async Task<ApiResponse<TableResponseDto>> GetTableByIdAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<TableResponseDto>.Fail("Kullanıcı bulunamadı.");
            var table = await _tableRepository.Query().FirstOrDefaultAsync(t => t.Id == id && t.IsActive == true);

            if (table == null)
                return ApiResponse<TableResponseDto>.Fail("Masa bulunamadı.");
            if (table.RestaurantId != user.Employee.RestaurantId)
                return ApiResponse<TableResponseDto>.Fail("Yetkisiz erişim.");
            var tableDto = _mapper.Map<TableResponseDto>(table);
            return ApiResponse<TableResponseDto>.Ok(tableDto, "Başarılı.");
        }
        public async Task<ApiResponse<IEnumerable<TableResponseDto>>> GetAllTablesByRestaurantIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<IEnumerable<TableResponseDto>>.Fail("Kullanıcı bulunamadı.");
            var tables = await _tableRepository.FindAsync(t => t.RestaurantId == user.Employee.RestaurantId && t.IsActive == true);
            var tableDtos = _mapper.Map<IEnumerable<TableResponseDto>>(tables);
            return ApiResponse<IEnumerable<TableResponseDto>>.Ok(tableDtos, "Başarılı.");
        }
        public async Task<ApiResponse<string>> GetTableCountByRestaurantIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<string>.Fail("Kullanıcı bulunamadı.");
            var count = await _tableRepository.CountAsync(t => t.RestaurantId == user.Employee.RestaurantId && t.IsActive == true);
            return ApiResponse<string>.Ok(count.ToString(), "Başarılı.");
        }
        public async Task<ApiResponse<NoContent>> CreateTableAsync(CreateTableRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            var entity = await _tableRepository.GetFirstOrDefaultAsync(t => t.Number == req.Number && t.IsActive == true);
            if(entity != null)
                return ApiResponse<NoContent>.Fail("Aynı masa numarasını kullanamazsınız.");
            var table = _mapper.Map<Table>(req);
            table.RestaurantId = user.Employee.RestaurantId;
            await _tableRepository.AddAsync(table);
            await _hubContext.Clients.Group(table.RestaurantId.ToString()).SendAsync("TableChanged");
            return ApiResponse<NoContent>.Ok("Masa başarıyla oluşturuldu.");
        }
        public async Task<ApiResponse<NoContent>> UpdateTableAsync(UpdateTableRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            var table = await _tableRepository.GetByIdAsync(req.Id);
            if (table == null)
                return ApiResponse<NoContent>.Fail("Masa bulunamadı.");
            _mapper.Map(req, table);
            var entity = await _tableRepository.GetFirstOrDefaultAsync(t => t.Number == req.Number && t.IsActive == true);
            if (entity != null)
                return ApiResponse<NoContent>.Fail("Aynı masa numarasını kullanamazsınız.");
            if (table.RestaurantId != user.Employee.RestaurantId)
                return ApiResponse<NoContent>.Fail("Yetkisiz erişim.");
            await _tableRepository.UpdateAsync(table);
            await _hubContext.Clients.Group(table.RestaurantId.ToString()).SendAsync("TableChanged");
            return ApiResponse<NoContent>.Ok("Masa başarıyla güncellendi.");
        }
        public async Task<ApiResponse<NoContent>> DeleteTableAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            var table = await _tableRepository.Query().Include(o => o.Orders).FirstOrDefaultAsync(t => t.Id == id);
            if (table == null)
                return ApiResponse<NoContent>.Fail("Masa bulunamadı.");
            if (table.RestaurantId != user.Employee.RestaurantId)
                return ApiResponse<NoContent>.Fail("Yetkisiz erişim.");
            if(table.Orders.Any(o => o.Status != "Completed" && o.Status != "Cancelled"))
                return ApiResponse<NoContent>.Fail("Masanın aktif siparişi bulunmaktadır!");
            table.IsActive = false;
            await _tableRepository.UpdateAsync(table);
            await _hubContext.Clients.Group(table.RestaurantId.ToString()).SendAsync("TableChanged");
            return ApiResponse<NoContent>.Ok("Masa başarıyla silindi.");
        }
        public async Task<ApiResponse<IEnumerable<TableResponseDto>>> GetTablesByRestaurantIdAndStatusAsync(string status)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<IEnumerable<TableResponseDto>>.Fail("Kullanıcı bulunamadı.");
            var tables = await _tableRepository.FindAsync(t => t.RestaurantId == user.Employee.RestaurantId && t.Status == status && t.IsActive == true);
            var tableDtos = _mapper.Map<IEnumerable<TableResponseDto>>(tables);
            return ApiResponse<IEnumerable<TableResponseDto>>.Ok(tableDtos, "Başarılı.");
        }
        public async Task<ApiResponse<string>> GetTableCountByRestaurantIdAndStatusAsync(string status)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<string>.Fail("Kullanıcı bulunamadı.");
            var count = await _tableRepository.CountAsync(t => t.RestaurantId == user.Employee.RestaurantId && t.Status == status && t.IsActive == true);
            return ApiResponse<string>.Ok(count.ToString(), "Başarılı.");
        }
        public async Task<ApiResponse<NoContent>> UpdateTableStatusByRestaurantIdAndIdAsync(UpdateTableStatusRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            var table = await _tableRepository.Query().Include(o => o.Orders).FirstOrDefaultAsync(t => t.Id == req.Id && t.RestaurantId == user.Employee.RestaurantId);
            if (table == null)
                return ApiResponse<NoContent>.Fail("Masa bulunamadı.");
            if(table.Orders.Any(o => o.Status != "Completed" && o.Status != "Cancelled") && req.Status == "Available")
                return ApiResponse<NoContent>.Fail("Bu masanın aktif bir siparişi var.");
            table.Status = req.Status;
            await _tableRepository.UpdateAsync(table);
            await _hubContext.Clients.Group(table.RestaurantId.ToString()).SendAsync("TableChanged");
            return ApiResponse<NoContent>.Ok("Masa durumu başarıyla güncellendi.");
        }

    }
}

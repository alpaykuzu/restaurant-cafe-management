using AutoMapper;
using Core.Dtos.MenuItemDtos;
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
    public class MenuItemService : IMenuItemService
    {
        private readonly IGenericRepository<MenuItem> _menuItemRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IHubContext<RestaurantHub> _hubContext;
        public MenuItemService(IGenericRepository<MenuItem> menuItemRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IGenericRepository<User> userRepository, IHubContext<RestaurantHub> hubContext)
        {
            _menuItemRepository = menuItemRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }
        public async Task<ApiResponse<IEnumerable<MenuItemResponsDto>>> GetAllMenuItemsByRestaurantIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<IEnumerable<MenuItemResponsDto>>.Fail("Kullanıcı bulunamadı.");
            }
            var menuItems = await _menuItemRepository.FindAsync(mi => mi.RestaurantId == user.Employee.RestaurantId && mi.IsActive == true);
            var menuItemDtos = _mapper.Map<IEnumerable<MenuItemResponsDto>>(menuItems);
            return ApiResponse<IEnumerable<MenuItemResponsDto>>.Ok(menuItemDtos, "Başarılı.");
        }
        public async Task<ApiResponse<IEnumerable<MenuItemResponsDto>>> GetMenuItemsByCategoryIdAsync(int categoryId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<IEnumerable<MenuItemResponsDto>>.Fail("Kullanıcı bulunamadı.");
            }
            var menuItems = await _menuItemRepository.FindAsync(c => c.CategoryId == categoryId && c.RestaurantId == user.Employee.RestaurantId && c.IsActive == true);
            if (menuItems == null)
            {
                return ApiResponse<IEnumerable<MenuItemResponsDto>>.Fail("Menü öğesi bulunamadı.");
            }
            var menuItemDtos = _mapper.Map<IEnumerable<MenuItemResponsDto>>(menuItems);
            return ApiResponse<IEnumerable<MenuItemResponsDto>>.Ok(menuItemDtos, "Başarılı.");
        }
        public async Task<ApiResponse<MenuItemResponsDto>> GetMenuItemByIdAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<MenuItemResponsDto>.Fail("Kullanıcı bulunamadı.");
            }
            var menuItem = await _menuItemRepository.GetFirstOrDefaultAsync(m => m.Id == id && m.RestaurantId == user.Employee.RestaurantId && m.IsActive == true);
            if (menuItem == null)
            {
                return ApiResponse<MenuItemResponsDto>.Fail("Menü öğesi bulunamadı.");
            }
            var menuItemDto = _mapper.Map<MenuItemResponsDto>(menuItem);
            return ApiResponse<MenuItemResponsDto>.Ok(menuItemDto, "Başarılı.");
        }
        public async Task<ApiResponse<NoContent>> CreateMenuItemAsync(CreateMenuItemRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            }
            var menuItem = _mapper.Map<MenuItem>(req);
            menuItem.RestaurantId = user.Employee.RestaurantId;
            await _menuItemRepository.AddAsync(menuItem);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("MenuItemChanged");
            return ApiResponse<NoContent>.Ok("Menü öğesi başarıyla oluşturuldu.");
        }
        public async Task<ApiResponse<NoContent>> UpdateMenuItemAsync(UpdateMenuItemRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            }
            var existingMenuItem = await _menuItemRepository.GetByIdAsync(req.Id);
            if (existingMenuItem == null)
            {
                return ApiResponse<NoContent>.Fail("Menü öğesi bulunamadı.");
            }
            if(existingMenuItem.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<NoContent>.Fail("Yetkisiz erişim.");
            }
            _mapper.Map(req, existingMenuItem);
            await _menuItemRepository.UpdateAsync(existingMenuItem);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("MenuItemChanged");
            return ApiResponse<NoContent>.Ok("Menü öğesi başarıyla güncellendi.");
        }
        public async Task<ApiResponse<NoContent>> UpdatePriceAsync(int id, int newPrice)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            }

            var existingMenuItem = await _menuItemRepository.GetByIdAsync(id);
            if (existingMenuItem == null)
            {
                return ApiResponse<NoContent>.Fail("Menü öğesi bulunamadı.");
            }
            if(existingMenuItem.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<NoContent>.Fail("Yetkisiz erişim.");
            }
            existingMenuItem.Price = newPrice;
            await _menuItemRepository.UpdateAsync(existingMenuItem);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("MenuItemChanged");
            return ApiResponse<NoContent>.Ok("Menü öğesi fiyatı başarıyla güncellendi.");
        }
        public async Task<ApiResponse<NoContent>> DeleteMenuItemAsync(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı bulunamadı.");
            }
            var existingMenuItem = await _menuItemRepository.GetByIdAsync(id);
            if (existingMenuItem == null)
            {
                return ApiResponse<NoContent>.Fail("Menü öğesi bulunamadı.");
            }
            if (existingMenuItem.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<NoContent>.Fail("Yetkisiz erişim.");
            }
            existingMenuItem.IsActive = false;
            await _menuItemRepository.UpdateAsync(existingMenuItem);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("MenuItemChanged");
            return ApiResponse<NoContent>.Ok("Menü öğesi başarıyla silindi.");
        }
    }
}

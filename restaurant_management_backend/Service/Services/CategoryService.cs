using AutoMapper;
using Core.Dtos.CategoryDtos;
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
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<RestaurantHub> _hubContext;
        private readonly IGenericRepository<MenuItem> _menuItemRepository;
        public CategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IGenericRepository<User> userRepository, IHubContext<RestaurantHub> hubContext, IGenericRepository<MenuItem> menuItemRepository)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _menuItemRepository = menuItemRepository;
        }
        public async Task<ApiResponse<IEnumerable<CategoryResponseDto>>> GetAllCategoryByRestaurantIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Employee == null || user.Employee.Restaurant == null)
            {
                return ApiResponse<IEnumerable<CategoryResponseDto>>.Fail("Kullanıcı veya restoran bulunamadı.");
            }
            var categories = await _categoryRepository.FindAsync(c => c.RestaurantId == user.Employee.RestaurantId && c.IsActive == true);
            var categoryDtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            return ApiResponse<IEnumerable<CategoryResponseDto>>.Ok(categoryDtos, "Kategorileri getirme başarılı.");
        }
        public async Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int categoryId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).ThenInclude(c => c.Categories).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Employee == null || user.Employee.Restaurant == null)
            {
                return ApiResponse<CategoryResponseDto>.Fail("Kullanıcı veya restoran bulunamadı.");
            }
            var category = _categoryRepository.Query().FirstOrDefault(c => c.Id == categoryId && c.RestaurantId == user.Employee.RestaurantId && c.IsActive == true);
            if (category == null)
            {
                return ApiResponse<CategoryResponseDto>.Fail("Kategori bulunamadı.");
            }
            var categoryDto = _mapper.Map<CategoryResponseDto>(category);
            return ApiResponse<CategoryResponseDto>.Ok(categoryDto, "Kategori getirme başarılı.");
        }
        public async Task<ApiResponse<NoContent>> CreateCategoryAsync(CreateCategoryRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Employee == null || user.Employee.Restaurant == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı veya restoran bulunamadı.");
            }
            var category = _mapper.Map<Category>(req);
            category.RestaurantId = user.Employee.RestaurantId;
            await _categoryRepository.AddAsync(category);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("CategoryChanged");
            return ApiResponse<NoContent>.Ok("Kategori oluşturma başarılı.");
        }
        public async Task<ApiResponse<NoContent>> UpdateCategoryAsync(UpdateCategoryRequestDto req)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Employee == null || user.Employee.Restaurant == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı veya restoran bulunamadı.");
            }
            var existingCategory = await _categoryRepository.GetByIdAsync(req.Id);
            if (existingCategory == null)
            {
                return ApiResponse<NoContent>.Fail("Kategori bulunamadı.");
            }
            _mapper.Map(req, existingCategory);
            if (existingCategory.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<NoContent>.Fail("Kategori güncelleme yetkiniz yok.");
            }
            await _categoryRepository.UpdateAsync(existingCategory);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("CategoryChanged");
            return ApiResponse<NoContent>.Ok("Kategori güncelleme başarılı.");
        }
        public async Task<ApiResponse<NoContent>> DeleteCategoryAsync(int categoryId)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.Employee == null || user.Employee.Restaurant == null)
            {
                return ApiResponse<NoContent>.Fail("Kullanıcı veya restoran bulunamadı.");
            }
            var existingCategory = await _categoryRepository.Query().Include(m => m.MenuItems).FirstOrDefaultAsync(c => c.Id ==categoryId);
            if (existingCategory == null)
            {
                return ApiResponse<NoContent>.Fail("Kategori bulunamadı.");
            }
            if (existingCategory.RestaurantId != user.Employee.RestaurantId)
            {
                return ApiResponse<NoContent>.Fail("Kategori silme yetkiniz yok.");
            }
            existingCategory.IsActive = false;
            foreach(var menuitem in existingCategory.MenuItems)
            {
                menuitem.IsActive = false;
                await _menuItemRepository.UpdateAsync(menuitem);
            }
            await _categoryRepository.UpdateAsync(existingCategory);
            await _hubContext.Clients.Group(user.Employee.RestaurantId.ToString()).SendAsync("CategoryChanged");
            return ApiResponse<NoContent>.Ok("Kategori silme başarılı.");
        }
    }
}

using AutoMapper;
using Core.Dtos.PaymentDtos;
using Core.Dtos.RestaurantDtos;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IGenericRepository<Restaurant> _restaurantRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RestaurantService(IGenericRepository<Restaurant> restaurantRepository, IMapper mapper, IGenericRepository<User> userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<RestaurantResponseDto>> GetRestaurantByUserIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<RestaurantResponseDto>.Fail("Kullanıcı bulunamadı.");
            var restaurant = await _restaurantRepository.GetFirstOrDefaultAsync(r => r.Id == user.Employee.RestaurantId);
            if (restaurant == null)
                return ApiResponse<RestaurantResponseDto>.Fail("Restorant bulunamadı.");
            var restaurantDto = _mapper.Map<RestaurantResponseDto>(restaurant);
            return ApiResponse<RestaurantResponseDto>.Ok(restaurantDto, "Başarılı.");
        }
        public async Task<ApiResponse<IEnumerable<RestaurantResponseDto>>> GetAllRestaurantsAsync()
        {
            var restaurants = await _restaurantRepository.GetAllAsync();
            var restauranDtos = _mapper.Map<IEnumerable<RestaurantResponseDto>>(restaurants);
            return ApiResponse<IEnumerable<RestaurantResponseDto>>.Ok(restauranDtos, "Başarılı.");
        }
        public async Task<ApiResponse<NoContent>> CreateRestaurantAsync(CreateRestaurantRequestDto req)
        {
            var newRestaurant = _mapper.Map<Restaurant>(req);
            await _restaurantRepository.AddAsync(newRestaurant);
            return ApiResponse<NoContent>.Ok("Restorant başarıyla oluşturuldu.");
        }
        public async Task<ApiResponse<NoContent>> UpdateRestaurantAsync(UpdateRestaurantRequestDto req)
        {
            var existingRestaurant = await _restaurantRepository.GetByIdAsync(req.Id);
            if (existingRestaurant == null)
            {
                return ApiResponse<NoContent>.Fail("Restorant bulunamadı.");
            }
            await _restaurantRepository.UpdateAsync(existingRestaurant);
            return ApiResponse<NoContent>.Ok("Restorant başarıyla güncellendi.");
        }
        public async Task<ApiResponse<NoContent>> DeleteRestaurantAsync(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null)
            {
                return ApiResponse<NoContent>.Fail("Restorant bulunamadı.");
            }
            await _restaurantRepository.DeleteAsync(restaurant);
            return ApiResponse<NoContent>.Ok("Restorant başarıyla silindi.");
        }
    }
}

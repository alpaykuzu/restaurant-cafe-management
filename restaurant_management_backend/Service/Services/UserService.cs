using AutoMapper;
using Core.Dtos.AuthDtos;
using Core.Dtos.TableDtos;
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
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IGenericRepository<User> userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<IEnumerable<UserResponseDto>>> GetAllUserAsync()
        {
            var users = await _userRepository.Query().Include(u => u.Roles).ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);

            foreach (var dto in userDtos)
            {
                var user = users.Find(u => u.Email == dto.Email);
                if (user != null && user.Roles != null)
                {
                    dto.Roles = user.Roles.Select(role => role.Name).ToList();
                }
                else
                {
                    dto.Roles = new List<string>();
                }
            }

            return ApiResponse<IEnumerable<UserResponseDto>>.Ok(userDtos, "Başarılı.");
        }
        public async Task<ApiResponse<UserResponseDto>> GetUserByIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserResponseDto>.Fail("Kullanıcı bulunamadı.");
            var userDto = _mapper.Map<UserResponseDto>(user);
            userDto.Roles = user.Roles.Select(role => role.Name).ToList();
            return ApiResponse<UserResponseDto>.Ok(userDto, "Başarılı.");
        }

        public async Task<ApiResponse<IEnumerable<UserResponseDto>>> GetManagersAsync()
        {
            var managers = await _userRepository.Query()
                .Where(u => u.Roles.Any(r => r.Name == "Manager"))
                .ToListAsync();
            var managerDto = _mapper.Map<IEnumerable<UserResponseDto>>(managers);
            return ApiResponse<IEnumerable<UserResponseDto>>.Ok(managerDto, "Başarılı.");
        }
    }
}

using AutoMapper;
using Core.Dtos.PaymentDtos;
using Core.Dtos.RoleDtos;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepository;
        public RoleService(IGenericRepository<Role> roleRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IGenericRepository<User> userRepository)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }
        public async Task<ApiResponse<IEnumerable<RoleResponseDto>>> GetAllRolesByRestaurantIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query().Include(u => u.Employee).ThenInclude(e => e.Restaurant).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponse<IEnumerable<RoleResponseDto>>.Fail("Kullanıcı bulunamadı.");
            var roles = await _roleRepository.FindAsync(r => r.User.Employee.RestaurantId == user.Employee.RestaurantId);
            var rolesDto = _mapper.Map<IEnumerable<RoleResponseDto>>(roles);
            return ApiResponse<IEnumerable<RoleResponseDto>>.Ok(rolesDto, "Başarılı.");
        }
        public async Task<ApiResponse<RoleResponseDto>> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return ApiResponse<RoleResponseDto>.Fail("Rol bulunamadı.");
            }
            var roleDto = _mapper.Map<RoleResponseDto>(role);
            return ApiResponse<RoleResponseDto>.Ok(roleDto, "Başarılı.");
        }
        public async Task<ApiResponse<IEnumerable<RoleResponseDto>>> GetAllRoleAsync()
        {            
            var roles = await _roleRepository.GetAllAsync();
            var rolesDto = _mapper.Map<IEnumerable<RoleResponseDto>>(roles);
            return ApiResponse<IEnumerable<RoleResponseDto>>.Ok(rolesDto, "Başarılı.");
        }
        public async Task<ApiResponse<RoleResponseDto>> CreateRoleAsync(CreateRoleRequestDto req)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (req.Name == "Admin" &&
               (user == null || !user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin")))
            {
                return ApiResponse<RoleResponseDto>.Fail("Yalnızca Admin rolüne sahip kullanıcılar Admin rolü oluşturabilir.");
            }
            var role = _mapper.Map<Role>(req);
            await _roleRepository.AddAsync(role);
            var createdRoleDto = _mapper.Map<RoleResponseDto>(role);
            return ApiResponse<RoleResponseDto>.Ok(createdRoleDto, "Rol başarıyla oluşturuldu.");
        }
        public async Task<ApiResponse<RoleResponseDto>> UpdateRoleAsync(UpdateRoleRequestDto req)
        {
            var existingRole = await _roleRepository.GetByIdAsync(req.Id);
            if (existingRole == null)
            {
                return ApiResponse<RoleResponseDto>.Fail("Rol bulunamadı.");
            }
            _mapper.Map(req, existingRole);
            await _roleRepository.UpdateAsync(existingRole);
            var updatedRoleDto = _mapper.Map<RoleResponseDto>(existingRole);
            return ApiResponse<RoleResponseDto>.Ok(updatedRoleDto, "Rol başarıyla güncellendi.");
        }
        public async Task<ApiResponse<NoContent>> DeleteRoleAsync(DeleteRoleRequestDto req)
        {
            var existingRole = await _roleRepository.GetFirstOrDefaultAsync(r => r.UserId == req.UserId && r.Name.Equals(req.Role));
            if (existingRole == null)
            {
                return ApiResponse<NoContent>.Fail("Rol bulunamadı.");
            }
            await _roleRepository.DeleteAsync(existingRole);
            return ApiResponse<NoContent>.Ok("Rol başarıyla silindi.");
        }
    }
}

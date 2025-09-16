using AutoMapper;
using Azure;
using Core.Dtos.AuthDtos;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IGenericRepository<User> userRepository, IGenericRepository<RefreshToken> refreshTokenRepository, IGenericRepository<Role> roleRepository, ITokenService tokenService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(u => u.Email == loginRequestDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.PasswordHash))
                return ApiResponse<LoginResponseDto>.Fail("Giriş başarısız");

            var rolesResult = await _roleRepository.FindAsync(r => r.UserId == user.Id);
            var roles = rolesResult.Select(x => x.Name);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new(ClaimTypes.Email, user.Email)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var accessTokenDto = _tokenService.GenerateToken(claims);
            var refreshTokenDto = _tokenService.GenerateRefreshToken();

            var existingToken = await _refreshTokenRepository.GetFirstOrDefaultAsync(x => x.UserId == user.Id);
            if (existingToken != null)
            {
                existingToken.Token = refreshTokenDto.RefreshToken;
                existingToken.ExpirationDate = refreshTokenDto.RefreshTokenExpTime;
                await _refreshTokenRepository.UpdateAsync(existingToken);
            }
            else
            {
                var refreshToken = new RefreshToken
                {
                    Token = refreshTokenDto.RefreshToken,
                    ExpirationDate = refreshTokenDto.RefreshTokenExpTime,
                    UserId = user.Id
                };
                await _refreshTokenRepository.AddAsync(refreshToken);
            }

            var userResponse = _mapper.Map<User, UserResponseDto>(user);
            userResponse.Roles = roles.ToList();

            var loginResponse = new LoginResponseDto
            {
                UserInfo = userResponse,
                AccessToken = accessTokenDto,
                RefreshToken = refreshTokenDto
            };

            return ApiResponse<LoginResponseDto>.Ok(loginResponse, "Giriş başarılı");
        }

        public async Task<ApiResponse<UserResponseDto>> MeAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _userRepository.Query()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return ApiResponse<UserResponseDto>.Fail("Kullanıcı bulunamadı");

            var roles = user.Roles.Select(x => x.Name).ToList();

            var userResponse = _mapper.Map<User, UserResponseDto>(user);
            userResponse.Roles = roles;

            return ApiResponse<UserResponseDto>.Ok(userResponse, "Kullanıcı bilgisi getirildi");
        }


        public async Task<ApiResponse<TokensResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto tokenRequest)
        {
            var existingToken = await _refreshTokenRepository.Query()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

            if (existingToken == null || existingToken.ExpirationDate < DateTime.UtcNow)
            {
                return ApiResponse<TokensResponseDto>.Fail("Geçersiz veya tarihi geçmiş token!");
            }

            var rolesResult = await _roleRepository.FindAsync(t => t.UserId == existingToken.User.Id);
            var roles = rolesResult.Select(x => x.Name);

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, existingToken.User.Id.ToString()),
                new (ClaimTypes.Name, existingToken.User.FirstName + " " + existingToken.User.LastName),
                new (ClaimTypes.Email, existingToken.User.Email)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var newGeneratedTokenDto = _tokenService.GenerateToken(claims);
            var newGeneratedRefreshTokenDto = _tokenService.GenerateRefreshToken();

            existingToken.Token = newGeneratedRefreshTokenDto.RefreshToken;
            existingToken.ExpirationDate = newGeneratedRefreshTokenDto.RefreshTokenExpTime;
            await _refreshTokenRepository.UpdateAsync(existingToken);

            var tokensResponse = new TokensResponseDto
            {
                AccessToken = newGeneratedTokenDto,
                RefreshToken = newGeneratedRefreshTokenDto,
                Roles = roles
            };

            return ApiResponse<TokensResponseDto>.Ok(tokensResponse, "Token güncellendi");
        }

        public async Task<ApiResponse<UserResponseDto>> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (await _userRepository.GetFirstOrDefaultAsync(u => u.Email == registerRequest.Email) != null)
                return ApiResponse<UserResponseDto>.Fail("Zaten kayıtlı hesap!");

            var user = _mapper.Map<RegisterRequestDto, User>(registerRequest, opt =>
            {
                opt.Items["PasswordHash"] = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
            });

            await _userRepository.AddAsync(user);
            await _roleRepository.AddAsync(new Role{ UserId = user.Id, Name = "User" });

            var userDto = _mapper.Map<UserResponseDto>(user);
            userDto.Roles = user.Roles.Select(u => u.Name).ToList();
            return ApiResponse<UserResponseDto>.Ok(userDto, "Kayıt başarılı");
        }
    }
}

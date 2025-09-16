using Core.Dtos.TokenDtos;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class TokenService : ITokenService
    {
        public readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AccessTokenResponseDto GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int accessTokenMinutes = int.Parse(_configuration["Jwt:AccessTokenMinutes"]); 
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(accessTokenMinutes),
                signingCredentials: creds);

            var accessToken = new AccessTokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpTime = DateTime.Now.AddMinutes(accessTokenMinutes)
            };
            return accessToken;
        }

        public RefreshTokenResponseDto GenerateRefreshToken()
        {
            Guid myuuid = Guid.NewGuid();
            var myuuidAsString = myuuid.ToString();
            int refreshTokenDays = int.Parse(_configuration["Jwt:RefreshTokenDays"]);
            var refreshToken = new RefreshTokenResponseDto
            {
                RefreshToken = myuuidAsString,
                RefreshTokenExpTime = DateTime.Now.AddDays(refreshTokenDays)
            };
            return refreshToken;
        }
    }
}


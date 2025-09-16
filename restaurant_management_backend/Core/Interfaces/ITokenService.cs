using Core.Dtos.TokenDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITokenService
    {
        public AccessTokenResponseDto GenerateToken(IEnumerable<Claim> claim);
        public RefreshTokenResponseDto GenerateRefreshToken();
    }
}

using Core.Dtos.TokenDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.AuthDtos
{
    public class LoginResponseDto
    {
        public UserResponseDto UserInfo { get; set; }
        public AccessTokenResponseDto AccessToken { get; set; }
        public RefreshTokenResponseDto RefreshToken { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.TokenDtos
{
    public class RefreshTokenResponseDto
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpTime { get; set; }
    }
}

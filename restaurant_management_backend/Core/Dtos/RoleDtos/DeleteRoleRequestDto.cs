using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.RoleDtos
{
    public class DeleteRoleRequestDto
    {
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}

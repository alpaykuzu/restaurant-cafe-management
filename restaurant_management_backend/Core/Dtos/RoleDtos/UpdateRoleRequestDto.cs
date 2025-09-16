using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.RoleDtos
{
    public class UpdateRoleRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } //user, admin, manager, kitchen,waiter, cashier
        public int UserId { get; set; }
    }
}

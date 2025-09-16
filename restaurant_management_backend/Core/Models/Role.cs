using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } //User, Admin, Manager, Kitchen,Waiter, Cashier
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

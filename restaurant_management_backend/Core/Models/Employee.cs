using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
        public Restaurant Restaurant { get; set; }
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<InventoryItem> InventoryItems { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Table> Tables { get; set; }
        public ICollection<SalesReport> SalesReports { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}

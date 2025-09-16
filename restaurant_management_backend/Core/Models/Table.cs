using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int Number { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } //Available, Occupied, Reserved
        public bool IsActive { get; set; } 
        public Restaurant Restaurant { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}

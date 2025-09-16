using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int TableId { get; set; }
        public int EmployeeId { get; set; }
        public int OrderNumber { get; set; }
        public string Status { get; set; } //"Pending", "Preparing", "Ready", "Served", "Completed", "Cancelled"
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public Restaurant Restaurant { get; set; }
        public Table Table { get; set; }
        public Employee Employee { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Invoice Invoice { get; set; }
        public Payment Payment { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OrderDtos
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int EmployeeId { get; set; }
        public int OrderNumber { get; set; }
        public string Status { get; set; } //"Pending", "Preparing", "Ready", "Served", "Completed", "Cancelled"
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public IEnumerable<OrderItemResponseDto> OrderItems { get; set; }
    }
}

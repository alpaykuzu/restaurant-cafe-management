using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OrderDtos
{
    public class CreateOrderItemRequestDto
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        //public decimal UnitPrice { get; set; }
    }
}

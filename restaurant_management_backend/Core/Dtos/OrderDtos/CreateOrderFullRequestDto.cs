using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OrderDtos
{
    public class CreateOrderFullRequestDto
    {
        public CreateOrderRequestDto Order { get; set; }
        public IEnumerable<CreateOrderItemRequestDto> OrderItems { get; set; }
    }
}

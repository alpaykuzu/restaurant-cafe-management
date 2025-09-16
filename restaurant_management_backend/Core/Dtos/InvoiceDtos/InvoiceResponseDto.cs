using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.InvoiceDtos
{
    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public DateTime IssuedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<InvoiceItemDto> Items { get; set; }
    }
}

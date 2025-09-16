using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public DateTime IssuedAt { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<InvoiceItem> Items { get; set; }
    }
}

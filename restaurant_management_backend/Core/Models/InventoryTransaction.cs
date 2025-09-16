using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int QuantityChanged { get; set; }
        public string Reason { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public Employee Employee { get; set; }
    }
}

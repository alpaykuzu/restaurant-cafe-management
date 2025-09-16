using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public int StockLevel { get; set; }
        public int MinimumStockLevel { get; set; }
        public int Unit { get; set; }
        public decimal Cost { get; set; }
        public DateTime LastUpdated { get; set; }
        public Restaurant Restaurant { get; set; }
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.InventoryItemDtos
{
    public class UpdateInventoryItemRequestDto
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public int StockLevel { get; set; }
        public int MinimumStockLevel { get; set; }
        public int Unit { get; set; }
        public decimal Cost { get; set; }
    }
}

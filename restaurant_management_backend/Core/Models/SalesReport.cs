using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class SalesReport
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        //public ICollection<MenuItem> PopularItems { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}

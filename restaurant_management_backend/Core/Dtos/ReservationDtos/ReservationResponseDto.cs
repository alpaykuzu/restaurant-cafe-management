using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.ReservationDtos
{
    public class ReservationResponseDto
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int TableId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public DateTime ReservationTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string Status { get; set; }
        public string SpecialRequests { get; set; }
    }
}

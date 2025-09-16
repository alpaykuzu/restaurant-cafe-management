using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.ReservationDtos
{
    public class UpdateReservationRequestDto
    {
        public int Id { get; set; }
        public DateTime ReservationTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string SpecialRequests { get; set; }
        public string Status { get; set; } // Confirmed, Cancelled, Completed
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.PaymentDtos
{
    public class CreatePaymentRequestDto
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } // Kart, Nakit, QR vs.
    }
}

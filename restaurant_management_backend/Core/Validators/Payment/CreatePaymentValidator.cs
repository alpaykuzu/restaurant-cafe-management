using Core.Dtos.AuthDtos;
using Core.Dtos.PaymentDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.Payment
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequestDto>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.OrderId).NotNull().WithMessage("Sipariş girilmesi zorunludur.");

            RuleFor(x => x.PaymentMethod).NotNull().WithMessage("Ödeme yöntemi girilmesi zorunludur.")
                .Must(p => new[] { "QR", "Cash", "Card" }.Contains(p))
                .WithMessage("Geçersiz ödeme yöntemi.");
        }
    }
}

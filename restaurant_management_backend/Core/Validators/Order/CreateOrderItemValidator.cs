using Core.Dtos.OrderDtos;
using FluentValidation;

namespace Core.Validators.Order
{
    public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemRequestDto>
    {
        public CreateOrderItemValidator()
        {
            RuleFor(x => x.MenuItemId).NotNull().WithMessage("Menu öğesi girilmesi zorunludur.");
            RuleFor(x => x.Quantity).NotNull().WithMessage("Adet girilmesi zorunludur.")
                .GreaterThan(0).WithMessage("Adet 0'dan büyük olmalı.");
        }
    }
}

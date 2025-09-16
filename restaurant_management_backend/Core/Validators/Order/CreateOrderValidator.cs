using Core.Dtos.AuthDtos;
using Core.Dtos.OrderDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.Order
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderRequestDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.TableId).NotNull().WithMessage("Masa girilmesi zorunludur.");
        }
    }
}

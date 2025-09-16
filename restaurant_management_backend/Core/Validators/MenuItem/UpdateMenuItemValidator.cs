using Core.Dtos.AuthDtos;
using Core.Dtos.MenuItemDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.MenuItem
{
    public class UpdateMenuItemValidator : AbstractValidator<UpdateMenuItemRequestDto>
    {
        public UpdateMenuItemValidator()
        {
            RuleFor(x => x.CategoryId).NotNull().WithMessage("Kategori girilmesi zorunludur.");
            RuleFor(x => x.Name).NotNull().WithMessage("İsim girilmesi zorunludur.");
            RuleFor(x => x.Description).NotNull().WithMessage("Açıklama girilmesi zorunludur.");
            RuleFor(x => x.Price).NotNull().WithMessage("Fiyat girilmesi zorunludur.")
                .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalı.");
        }
    }
}

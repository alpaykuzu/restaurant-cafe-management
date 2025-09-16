using Core.Dtos.AuthDtos;
using Core.Dtos.TableDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.Table
{
    public class CreateTableValidator : AbstractValidator<CreateTableRequestDto>
    {
        public CreateTableValidator()
        {
            RuleFor(x => x.Number).NotNull().WithMessage("Masa numarası girilmesi zorunludur.")
                .GreaterThan(0).WithMessage("Masa numarası 0'dan büyük olmalı");

            RuleFor(x => x.Capacity).NotNull().WithMessage("Kapasite girilmesi zorunludur.")
                .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalı");

            RuleFor(x => x.Status).NotNull().WithMessage("Masa durumu girilmesi zorunludur.")
                .Must(p => new[] { "Available", "Occupied", "Reserved" }.Contains(p))
                .WithMessage("Geçersiz durum.");
        }
    }
}

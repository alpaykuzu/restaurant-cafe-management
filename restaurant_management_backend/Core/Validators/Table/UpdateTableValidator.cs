using Core.Dtos.TableDtos;
using FluentValidation;

namespace Core.Validators.Table
{
    public class UpdateTableValidator : AbstractValidator<UpdateTableRequestDto>
    {
        public UpdateTableValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Id girilmesi zorunludur.");

            RuleFor(x => x.Number).NotNull().WithMessage("MAsa numarası girilmesi zorunludur.")
                .GreaterThan(0).WithMessage("Masa numarası 0'dan büyük olmalı");

            RuleFor(x => x.Capacity).NotNull().WithMessage("Kapasite girilmesi zorunludur.")
                .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalı");

            RuleFor(x => x.Status).NotNull().WithMessage("Masa durumu girilmesi zorunludur.")
                .Must(p => new[] { "Available", "Occupied", "Reserved" }.Contains(p))
                .WithMessage("Geçersiz durum.");
        }
    }
}

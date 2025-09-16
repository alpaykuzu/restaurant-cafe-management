using Core.Dtos.TableDtos;
using FluentValidation;

namespace Core.Validators.Table
{
    public class UpdateTableStatusValidator : AbstractValidator<UpdateTableStatusRequestDto>
    {
        public UpdateTableStatusValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Id girilmesi zorunludur.");

            RuleFor(x => x.Status).NotNull().WithMessage("Masa durumu girilmesi zorunludur.")
                .Must(p => new[] { "Available", "Occupied", "Reserved" }.Contains(p))
                .WithMessage("Geçersiz durum.");
        }
    }
}

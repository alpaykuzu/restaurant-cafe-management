using Core.Dtos.RestaurantDtos;
using FluentValidation;

namespace Core.Validators.Restaurant
{
    public class UpdateRestaurantValidator : AbstractValidator<UpdateRestaurantRequestDto>
    {
        public UpdateRestaurantValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Id girilmesi zorunludur.")
                .GreaterThan(0).WithMessage("Id 0'dan büyük olmalıdır.");

            RuleFor(x => x.Name).NotNull().WithMessage("İsim girilmesi zorunludur.")
                .MinimumLength(2).WithMessage("İsim en az 2 karakter olmalı.")
                .MaximumLength(30).WithMessage("İsim en fazla 30 karakter olmalı");

            RuleFor(x => x.Address).NotNull().WithMessage("Adres girilmesi zorunludur.")
                .MinimumLength(2).WithMessage("Adres en az 2 karakter olmalı.")
                .MaximumLength(100).WithMessage("Soyad en fazla 100 karakter olmalı");

            RuleFor(x => x.Email).NotNull().WithMessage("Email girilmesi zorunludur.").EmailAddress();

            RuleFor(x => x.Phone).NotNull().WithMessage("Telefon girilmesi zorunludur.");
        }
    }
}

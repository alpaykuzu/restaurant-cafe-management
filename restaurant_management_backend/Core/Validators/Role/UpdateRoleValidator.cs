using Core.Dtos.RoleDtos;
using FluentValidation;

namespace Core.Validators.Role
{
    public class UpdateRoleValidator : AbstractValidator<UpdateRoleRequestDto>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("İsim girilmesi zorunludur.")
                .Must(p => new[] { "Admin", "Manager", "Kitchen", "Cashier", "Waiter", "User" }.Contains(p))
                            .WithMessage("Geçersiz rol.");

            RuleFor(x => x.UserId).NotNull().WithMessage("Kullanıcı girilmesi zorunludur.");

            RuleFor(x => x.Id).NotNull().WithMessage("Id girilmesi zorunludur.");
        }
    }
}

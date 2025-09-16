using Core.Dtos.RoleDtos;
using FluentValidation;

namespace Core.Validators.Role
{
    public class DeleteRoleValidator : AbstractValidator<DeleteRoleRequestDto>
    {
        public DeleteRoleValidator()
        {
            RuleFor(x => x.Role).NotNull().WithMessage("İsim girilmesi zorunludur.")
                .Must(p => new[] { "Admin", "Manager", "Kitchen", "Cashier", "Waiter" , "User"}.Contains(p))
                            .WithMessage("Geçersiz rol.");

            RuleFor(x => x.UserId).NotNull().WithMessage("Kullanıcı girilmesi zorunludur.");
        }
    }
}

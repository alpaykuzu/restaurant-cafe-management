using Core.Dtos.AuthDtos;
using Core.Dtos.RoleDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.Role
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleRequestDto>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("İsim girilmesi zorunludur.")
                .Must(p => new[] { "Admin", "Manager", "Kitchen", "Cashier", "Waiter", "User" }.Contains(p))
                            .WithMessage("Geçersiz rol.");

            RuleFor(x => x.UserId).NotNull().WithMessage("Kullanıcı girilmesi zorunludur.");
        }
    }
}

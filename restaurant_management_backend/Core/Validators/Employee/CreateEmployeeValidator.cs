using Core.Dtos.AuthDtos;
using Core.Dtos.EmployeeDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.Employee
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeRequestDto>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.Salary).NotNull().WithMessage("Maaş girilmesi zorunludur.")
                .GreaterThan(0).WithMessage($"Maaş 0 TL'den az olamaz.");

            RuleFor(x => x.HireDate).NotNull().WithMessage("İşe giriş tarihi girilmesi zorunludur.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("İşe giriş tarihi bugünden ileri olamaz.");
        }
    }
}

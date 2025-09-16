using Core.Dtos.AuthDtos;
using Core.Dtos.SalesReportDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.SalesReport
{
    public class CreateSalesReportValidator : AbstractValidator<CreateSalesReportRequestDto>
    {
        public CreateSalesReportValidator()
        {
            RuleFor(x => x.StartDate).NotNull().WithMessage("Başlangıç tarihi girilmesi zorunludur.")
                 .LessThanOrEqualTo(DateTime.Now).WithMessage("Başlangıç tarihi bugünden ileri olamaz");

            RuleFor(x => x.EndDate).NotNull().WithMessage("Başlangıç tarihi girilmesi zorunludur.");
        }
    }
}

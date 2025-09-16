using Core.Dtos.CategoryDtos;
using Core.Dtos.EmployeeDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.Category
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequestDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("Kategori ismi girilmesi zorunludur.");

            RuleFor(x => x.Description).NotNull().WithMessage("Kategori açıklaması girilmesi zorunludur.");
        }
    }
}

using Core.Dtos.AuthDtos;
using Core.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators.Auth
{
    public class RegisterValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName).NotNull().WithMessage("İsim girilmesi zorunludur.")
                .MinimumLength(2).WithMessage("İsim en az 2 karakter olmalı.")
                .MaximumLength(15).WithMessage("İsim en fazla 30 karakter olmalı");

            RuleFor(x => x.LastName).NotNull().WithMessage("Soyad girilmesi zorunludur.")
                .MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalı.")
                .MaximumLength(15).WithMessage("Soyad en fazla 30 karakter olmalı");

            RuleFor(x => x.Email).NotNull().WithMessage("Email girilmesi zorunludur.").EmailAddress();

            RuleFor(x => x.Password).NotNull().WithMessage("Şifre girilmesi zorunludur.")
                .MinimumLength(6).MaximumLength(100).WithMessage("Şifre uzunluğu hatalı.");
        }
    }
}
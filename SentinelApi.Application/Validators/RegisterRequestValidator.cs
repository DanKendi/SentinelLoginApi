namespace SentinelApi.Application.Validators;

using FluentValidation;
using SentinelApi.Application.DTOs;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(150).WithMessage("E-mail deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(6).WithMessage("Senha deve ter pelo menos 6 caracteres.");

        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("FCM Token é obrigatório.");

        RuleFor(x => x.RaioKm)
            .GreaterThan(0).WithMessage("Raio deve ser maior que 0.")
            .LessThanOrEqualTo(500).WithMessage("Raio deve ser no máximo 500 km.");
    }
}
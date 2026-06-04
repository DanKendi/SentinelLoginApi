namespace SentinelApi.Application.Validators;

using FluentValidation;
using SentinelApi.Application.DTOs;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.RaioKm)
            .GreaterThan(0).WithMessage("Raio deve ser maior que 0.")
            .LessThanOrEqualTo(500).WithMessage("Raio deve ser no máximo 500 km.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90.")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180.")
            .When(x => x.Longitude.HasValue);
    }
}
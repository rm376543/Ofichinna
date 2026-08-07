using FluentValidation;
using Ofichina.Contracts.Requests.Veiculo;

namespace Ofichina.Application.Validators.Veiculo;

/// <summary>
/// Validador para atualização de veículo.
/// </summary>
public sealed class UpdateVeiculoRequestValidator : AbstractValidator<UpdateVeiculoRequest>
{
    public UpdateVeiculoRequestValidator()
    {
        RuleFor(x => x.VeiculoId)
            .NotEmpty().WithMessage("O Id do veículo é obrigatório.");

        RuleFor(x => x.PessoaId)
            .NotEmpty().WithMessage("A pessoa vinculada é obrigatória.");

        RuleFor(x => x.Placa)
            .NotEmpty().WithMessage("A placa é obrigatória.")
            .Length(7, 8).WithMessage("A placa deve ter entre 7 e 8 caracteres.");

        RuleFor(x => x.Marca)
            .NotEmpty().WithMessage("A marca é obrigatória.")
            .MaximumLength(100).WithMessage("A marca não pode exceder 100 caracteres.");

        RuleFor(x => x.Modelo)
            .NotEmpty().WithMessage("O modelo é obrigatório.")
            .MaximumLength(100).WithMessage("O modelo não pode exceder 100 caracteres.");

        RuleFor(x => x.AnoFabricacao)
            .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
            .WithMessage("O ano de fabricação é inválido.");

        RuleFor(x => x.Cor)
            .MaximumLength(50).WithMessage("A cor não pode exceder 50 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Cor));

        RuleFor(x => x.Hodometro)
            .GreaterThanOrEqualTo(0).WithMessage("A quilometragem não pode ser negativa.");
    }
}
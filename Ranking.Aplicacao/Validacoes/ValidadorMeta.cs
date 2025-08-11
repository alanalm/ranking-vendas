using FluentValidation;
using Ranking.Aplicacao.DTOs;
using static Ranking.Aplicacao.Validacoes.ValidadorBase;

namespace Ranking.Aplicacao.Validacoes
{
    public class ValidadorCriarMeta : AbstractValidator<CriarMetaDto>
    {
        /// Construtor que define as regras de validação.
        public ValidadorCriarMeta()
        {
            RuleFor(m => m.IndicadorId)
                .GreaterThan(0)
                .WithMessage("O ID do indicador deve ser maior que zero.");

            RuleFor(m => m.Valor)
                .GreaterThan(0)
                .WithMessage("O valor da meta deve ser maior que zero.")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("O valor da meta não pode exceder 999.999.999,99.");

            RuleFor(m => m.DataInicio)
                .NotEmpty()
                .WithMessage("A data de início é obrigatória.")
                .Must(ValidacoesComuns.DataValidaOuNula)
                .WithMessage("A data de início deve ser uma data válida.");

            RuleFor(m => m.DataFim)
                .Must((meta, dataFim) => !dataFim.HasValue || dataFim.Value > meta.DataInicio)
                .WithMessage("A data de fim deve ser posterior à data de início.")
                .Must(ValidacoesComuns.DataValidaOuNula)
                .WithMessage("A data de fim deve ser uma data válida ou nula.");

        }
    }

    /// Validador para atualização de metas.
    /// Implementa regras de validação usando FluentValidation.
    public class ValidadorAtualizarMeta : AbstractValidator<AtualizarMetaDto>
    {
        /// Construtor que define as regras de validação.
        public ValidadorAtualizarMeta()
        {
            RuleFor(m => m.Id)
                .GreaterThan(0)
                .WithMessage("O ID da meta deve ser maior que zero.");

            RuleFor(m => m.IndicadorId)
                .GreaterThan(0)
                .WithMessage("O ID do indicador deve ser maior que zero.");

            RuleFor(m => m.Valor)
                .GreaterThan(0)
                .WithMessage("O valor da meta deve ser maior que zero.")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("O valor da meta não pode exceder 999.999.999,99.");

            RuleFor(m => m.DataInicio)
                .NotEmpty()
                .WithMessage("A data de início é obrigatória.")
                .Must(ValidacoesComuns.DataValidaOuNula)
                .WithMessage("A data de início deve ser uma data válida.");

            RuleFor(m => m.DataFim)
                .Must((meta, dataFim) => !dataFim.HasValue || dataFim.Value > meta.DataInicio)
                .WithMessage("A data de fim deve ser posterior à data de início.")
                .Must(ValidacoesComuns.DataValidaOuNula)
                .WithMessage("A data de fim deve ser uma data válida ou nula.");

            RuleFor(m => m.Ativa)
                .NotNull()
                .WithMessage("O status ativo da meta deve ser informado.");
        }
    }
}

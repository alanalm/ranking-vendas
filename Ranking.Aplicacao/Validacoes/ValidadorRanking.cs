using FluentValidation;
using Ranking.Aplicacao.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ranking.Aplicacao.Validacoes.ValidadorBase;

namespace Ranking.Aplicacao.Validacoes
{
    public class ValidadorFiltroRanking : AbstractValidator<FiltroRankingDto>
    {
        /// Construtor que define as regras de validação.
        public ValidadorFiltroRanking()
        {
            RuleFor(f => f.DataInicio)
                .Must(ValidacoesComuns.DataValidaOuNula)
                .WithMessage("A data de início deve ser uma data válida ou nula.")
                .Must((filtro, dataInicio) => !dataInicio.HasValue || !filtro.DataFim.HasValue || dataInicio.Value <= filtro.DataFim.Value)
                .WithMessage("A data de início deve ser anterior ou igual à data de fim.");

            RuleFor(f => f.DataFim)
                .Must(ValidacoesComuns.DataValidaOuNula)
                .WithMessage("A data de fim deve ser uma data válida ou nula.")
                .Must((filtro, dataFim) => !dataFim.HasValue || !filtro.DataInicio.HasValue || dataFim.Value >= filtro.DataInicio.Value)
                .WithMessage("A data de fim deve ser posterior ou igual à data de início.");

            RuleFor(f => f.TipoOrdenacao)
                .IsInEnum()
                .WithMessage("O tipo de ordenação deve ser um valor válido.");

            // Validação customizada para período máximo
            RuleFor(f => f)
                .Must(ValidacoesComuns.TemPeriodoValido)
                .WithMessage("O período entre as datas não pode exceder 5 anos.")
                .When(f => f.DataInicio.HasValue && f.DataFim.HasValue);
        }
    }
}

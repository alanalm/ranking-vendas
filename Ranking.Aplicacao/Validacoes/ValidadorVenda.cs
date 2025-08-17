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
    public class ValidadorCriarVenda : AbstractValidator<CriarVendaDto>
    {
        /// Construtor que define as regras de validação.
        public ValidadorCriarVenda()
        {
            RuleFor(v => v.FuncionarioId)
                .GreaterThan(0)
                .WithMessage("O ID do funcionário deve ser maior que zero.");

            RuleFor(v => v.Valor)
                .GreaterThan(0)
                .WithMessage("O valor da venda deve ser maior que zero.")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("O valor da venda não pode exceder 999.999.999,99.");

            RuleFor(v => v.DataVenda)
                .NotEmpty()
                .WithMessage("A data da venda é obrigatória.")
                .Must(ValidacoesComuns.DataValidaOuNula)
                .WithMessage("A data da venda deve estar entre 01/01/2000 e a data atual.");

            RuleFor(v => v.Descricao)
                 .NotEmpty().WithMessage("A descrição é obrigatória.")
                 .MaximumLength(500).WithMessage("A descrição não pode exceder 500 caracteres.");
        }
    }

    /// Validador para atualização de vendas.
    /// Implementa regras de validação usando FluentValidation.
    public class ValidadorAtualizarVenda : AbstractValidator<AtualizarVendaDto>
    {
        /// Construtor que define as regras de validação.
        public ValidadorAtualizarVenda()
        {
            RuleFor(v => v.Id)
                .GreaterThan(0)
                .WithMessage("O ID da venda deve ser maior que zero.");

            RuleFor(v => v.FuncionarioId)
                .GreaterThan(0)
                .WithMessage("O ID do funcionário deve ser maior que zero.");

            RuleFor(v => v.Valor)
                .GreaterThan(0)
                .WithMessage("O valor da venda deve ser maior que zero.")
                .LessThanOrEqualTo(999999999.99m)
                .WithMessage("O valor da venda não pode exceder 999.999.999,99.");

            RuleFor(v => v.DataVenda)
                .NotEmpty()
                .WithMessage("A data da venda é obrigatória.")
                .Must(ValidacoesComuns.DataValidaOuNula)
                .WithMessage("A data da venda deve estar entre 01/01/2000 e a data atual.");

            RuleFor(v => v.Descricao)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MaximumLength(500).WithMessage("A descrição não pode exceder 500 caracteres.");
        }
    }
}

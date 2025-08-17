using FluentValidation;
using Ranking.Aplicacao.DTOs;
using Ranking.Dominio.Enums;
using static Ranking.Aplicacao.Validacoes.ValidadorBase;

namespace Ranking.Aplicacao.Validacoes
{
    public class ValidadorCriarIndicador : AbstractValidator<CriarIndicadorDto>
    {
        public ValidadorCriarIndicador()
        {
            RuleFor(i => i.Nome)
                .NotEmpty()
                .WithMessage("O nome do indicador é obrigatório.")
                .Length(2, 100)
                .WithMessage("O nome deve ter entre 2 e 100 caracteres.")
                .Matches(ValidacoesComuns.RegexNome)
                .WithMessage("O nome deve conter apenas letras, números, espaços, hífens e underscores.");

            RuleFor(i => i.Descricao)
                .NotEmpty()
                .WithMessage("A descrição do indicador é obrigatória.")
                .Length(10, 500)
                .WithMessage("A descrição deve ter entre 10 e 500 caracteres.");

            RuleFor(i => i.Tipo)
                .NotEqual(TipoIndicador.Nenhum)
                .WithMessage("O tipo do indicador é obrigatório.");
        }
    }

    public class ValidadorAtualizarIndicador : AbstractValidator<AtualizarIndicadorDto>
    {
        public ValidadorAtualizarIndicador()
        {
            RuleFor(i => i.Id)
                .GreaterThan(0)
                .WithMessage("O ID do indicador deve ser maior que zero.");

            RuleFor(i => i.Nome)
                .NotEmpty()
                .WithMessage("O nome do indicador é obrigatório.")
                .Length(2, 100)
                .WithMessage("O nome deve ter entre 2 e 100 caracteres.")
                .Matches(ValidacoesComuns.RegexNome)
                .WithMessage("O nome deve conter apenas letras, números, espaços, hífens e underscores.");

            RuleFor(i => i.Descricao)
                .NotEmpty()
                .WithMessage("A descrição do indicador é obrigatória.")
                .Length(10, 500)
                .WithMessage("A descrição deve ter entre 10 e 500 caracteres.");

            RuleFor(i => i.Tipo)
                .NotEqual(TipoIndicador.Nenhum)
                .WithMessage("O tipo do indicador é obrigatório.");
        }
    }
}
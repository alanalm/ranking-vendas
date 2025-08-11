using FluentValidation;
using Ranking.Aplicacao.DTOs;
using static Ranking.Aplicacao.Validacoes.ValidadorBase;
namespace Ranking.Aplicacao.Validacoes
{
    public class ValidadorCriarFuncionario : AbstractValidator<CriarFuncionarioDto>
    {
        public ValidadorCriarFuncionario()
        {
            RuleFor(f => f.Nome)
                .NotEmpty()
                .WithMessage("O nome do funcionário é obrigatório.")
                .Length(2, 100)
                .WithMessage("O nome deve ter entre 2 e 100 caracteres.")
                .Matches(ValidacoesComuns.RegexNome)
                .WithMessage("O nome deve conter apenas letras e espaços.");
        }
    }

    public class ValidadorAtualizarFuncionario : AbstractValidator<AtualizarFuncionarioDto>
    {
        public ValidadorAtualizarFuncionario()
        {
            RuleFor(f => f.Id)
                .GreaterThan(0)
                .WithMessage("O ID do funcionário deve ser maior que zero.");

            RuleFor(f => f.Nome)
                .NotEmpty()
                .WithMessage("O nome do funcionário é obrigatório.")
                .Length(2, 100)
                .WithMessage("O nome deve ter entre 2 e 100 caracteres.")
                .Matches(ValidacoesComuns.RegexNome)
                .WithMessage("O nome deve conter apenas letras e espaços.");
        }
    }
}

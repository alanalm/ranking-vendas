using Aplicacao.Utils;
using AutoMapper;
using FluentValidation;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;

namespace Ranking.Aplicacao.Servicos
{
    public class FuncionarioServico : IFuncionarioServico
    {
        private readonly IRepositorioFuncionario _repositorioFuncionario;
        private readonly IRepositorioVenda _repositorioVenda;
        private readonly IValidator<CriarFuncionarioDto> _validadorCriacao;
        private readonly IValidator<AtualizarFuncionarioDto> _validadorAtualizacao;
        private readonly IMapper _mapper;

        public FuncionarioServico(
            IRepositorioFuncionario repositorioFuncionario,
            IRepositorioVenda repositorioVenda,
            IValidator<CriarFuncionarioDto> validadorCriacao,
            IValidator<AtualizarFuncionarioDto> validadorAtualizacao,
            IMapper mapper)
        {
            _repositorioFuncionario = repositorioFuncionario;
            _repositorioVenda = repositorioVenda;
            _validadorCriacao = validadorCriacao;
            _validadorAtualizacao = validadorAtualizacao;
            _mapper = mapper;
        }

        private FuncionarioDto Map(Funcionario funcionario) =>
            _mapper.Map<FuncionarioDto>(funcionario);

        private static List<string>? ValidarDto<T>(IValidator<T> validator, T dto)
        {
            var resultado = validator.Validate(dto);
            if (!resultado.IsValid)
                return resultado.Errors.Select(e => e.ErrorMessage).ToList();

            return null;
        }

        public async Task<ResultadoOperacao<IEnumerable<FuncionarioDto>>> ObterTodosAsync()
        {
            var funcionarios = await _repositorioFuncionario.ObterTodosAsync();
            return ResultadoOperacao<IEnumerable<FuncionarioDto>>.CriarSucesso(
                _mapper.Map<IEnumerable<FuncionarioDto>>(funcionarios)
            );
        }

        public async Task<ResultadoOperacao<FuncionarioDto>> ObterPorIdAsync(int id)
        {
            var funcionario = await _repositorioFuncionario.ObterPorIdAsync(id);
            return funcionario == null
                ? ResultadoOperacao<FuncionarioDto>.CriarFalha("Funcionário não encontrado.")
                : ResultadoOperacao<FuncionarioDto>.CriarSucesso(Map(funcionario));
        }

        public async Task<ResultadoOperacao<IEnumerable<FuncionarioDto>>> PesquisarPorNomeAsync(string nome)
        {
            var funcionarios = await _repositorioFuncionario.PesquisarPorNomeAsync(nome);
            return ResultadoOperacao<IEnumerable<FuncionarioDto>>.CriarSucesso(
                _mapper.Map<IEnumerable<FuncionarioDto>>(funcionarios)
            );
        }

        public async Task<ResultadoOperacao<FuncionarioDto>> CriarAsync(CriarFuncionarioDto dto)
        {
            var erros = ValidarDto(_validadorCriacao, dto);
            if (erros != null)
                return ResultadoOperacao<FuncionarioDto>.CriarFalha(erros);

            var funcionario = _mapper.Map<Funcionario>(dto);
            await _repositorioFuncionario.AdicionarAsync(funcionario);

            return ResultadoOperacao<FuncionarioDto>.CriarSucesso(Map(funcionario));
        }

        public async Task<ResultadoOperacao<FuncionarioDto>> AtualizarAsync(AtualizarFuncionarioDto dto)
        {
            var erros = ValidarDto(_validadorAtualizacao, dto);
            if (erros != null)
                return ResultadoOperacao<FuncionarioDto>.CriarFalha(erros);

            var funcionarioExistente = await _repositorioFuncionario.ObterPorIdAsync(dto.Id);
            if (funcionarioExistente == null)
                return ResultadoOperacao<FuncionarioDto>.CriarFalha($"Funcionário com ID {dto.Id} não encontrado.");

            _mapper.Map(dto, funcionarioExistente);
            await _repositorioFuncionario.AtualizarAsync(funcionarioExistente);

            return ResultadoOperacao<FuncionarioDto>.CriarSucesso(Map(funcionarioExistente));
        }


        public async Task<ResultadoOperacao<bool>> RemoverAsync(int id)
        {
            var funcionario = await _repositorioFuncionario.ObterPorIdAsync(id);
            if (funcionario == null)
                return ResultadoOperacao<bool>.CriarFalha("Funcionário não encontrado.");

            if (await _repositorioVenda.TemVendasAsync(id))
                return ResultadoOperacao<bool>.CriarFalha("Não é possível remover: funcionário possui vendas associadas.");

            if (!await _repositorioFuncionario.RemoverAsync(funcionario))
                return ResultadoOperacao<bool>.CriarFalha("Falha ao remover o funcionário.");

            return ResultadoOperacao<bool>.CriarSucesso(true);
        }

        public async Task<ResultadoOperacao<bool>> PodeSerRemovidoAsync(int id)
        {
            var temVendas = await _repositorioVenda.TemVendasAsync(id);
            return ResultadoOperacao<bool>.CriarSucesso(!temVendas);
        }

        public async Task<ResultadoOperacao<FuncionarioDto>> ObterPorNomeAsync(string nome)
        {
            var funcionario = await _repositorioFuncionario.ObterPorNomeAsync(nome);
            return funcionario == null
                ? ResultadoOperacao<FuncionarioDto>.CriarFalha("Funcionário não encontrado.")
                : ResultadoOperacao<FuncionarioDto>.CriarSucesso(Map(funcionario));
        }

        public async Task<ResultadoOperacao<IEnumerable<FuncionarioDto>>> ObterComEstatisticasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var funcionarios = await _repositorioFuncionario.ObterComVendasNoPeriodoAsync(
                dataInicio ?? DateTime.MinValue,
                dataFim ?? DateTime.MaxValue
            );
            return ResultadoOperacao<IEnumerable<FuncionarioDto>>.CriarSucesso(
                _mapper.Map<IEnumerable<FuncionarioDto>>(funcionarios)
            );
        }

        public async Task<ResultadoOperacao<bool>> JaExisteComNomeAsync(string nome, int? idExcluir = null)
        {
            var jaExiste = await _repositorioFuncionario.JaExisteComNomeAsync(nome, idExcluir);
            return ResultadoOperacao<bool>.CriarSucesso(jaExiste);
        }
    }

}

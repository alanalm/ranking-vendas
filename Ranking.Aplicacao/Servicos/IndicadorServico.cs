using Aplicacao.Utils;
using AutoMapper;
using FluentValidation;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Enums;
using Ranking.Dominio.Interfaces.Repositorios;

namespace Ranking.Aplicacao.Servicos
{
    public class IndicadorServico : IIndicadorServico
    {
        private readonly IRepositorioIndicador _repositorioIndicador;
        private readonly IRepositorioMeta _repositorioMeta;
        private readonly IValidator<CriarIndicadorDto> _validadorCriacao;
        private readonly IValidator<AtualizarIndicadorDto> _validadorAtualizacao;
        private readonly IMapper _mapper;

        public IndicadorServico(
            IRepositorioIndicador repositorioIndicador,
            IRepositorioMeta repositorioMeta,
            IValidator<CriarIndicadorDto> validadorCriacao,
            IValidator<AtualizarIndicadorDto> validadorAtualizacao,
            IMapper mapper)
        {
            _repositorioIndicador = repositorioIndicador;
            _repositorioMeta = repositorioMeta;
            _validadorCriacao = validadorCriacao;
            _validadorAtualizacao = validadorAtualizacao;
            _mapper = mapper;
        }

        private async Task<ResultadoOperacao<IndicadorDto>> ValidarDtoAsync<TDto>(IValidator<TDto> validator, TDto dto)
        {
            var validacao = await validator.ValidateAsync(dto);
            if (!validacao.IsValid)
                return ResultadoOperacao<IndicadorDto>.CriarFalha(
                    validacao.Errors.Select(e => e.ErrorMessage).ToList()
                );

            return null;
        }

        public async Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> ObterTodosAsync()
        {
            var indicadores = await _repositorioIndicador.ObterTodosAsync();
            return ResultadoOperacao<IEnumerable<IndicadorDto>>.CriarSucesso(
                _mapper.Map<IEnumerable<IndicadorDto>>(indicadores)
            );
        }

        public async Task<ResultadoOperacao<IndicadorDto>> ObterPorIdAsync(int id)
        {
            var indicador = await _repositorioIndicador.ObterPorIdAsync(id);
            return indicador == null
                ? ResultadoOperacao<IndicadorDto>.CriarFalha("Indicador não encontrado.")
                : ResultadoOperacao<IndicadorDto>.CriarSucesso(_mapper.Map<IndicadorDto>(indicador));
        }

        public async Task<ResultadoOperacao<IndicadorDto>> ObterPorNomeAsync(string nome)
        {
            var indicador = await _repositorioIndicador.ObterPorNomeAsync(nome);
            return indicador == null
                ? ResultadoOperacao<IndicadorDto>.CriarFalha("Indicador não encontrado.")
                : ResultadoOperacao<IndicadorDto>.CriarSucesso(_mapper.Map<IndicadorDto>(indicador));
        }

        public async Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> PesquisarPorNomeAsync(string nome)
        {
            var indicadores = await _repositorioIndicador.PesquisarPorNomeAsync(nome);
            return ResultadoOperacao<IEnumerable<IndicadorDto>>.CriarSucesso(
                _mapper.Map<IEnumerable<IndicadorDto>>(indicadores)
            );
        }

        public async Task<ResultadoOperacao<IndicadorDto>> CriarAsync(CriarIndicadorDto dto)
        {
            var resultadoValidacao = await ValidarDtoAsync(_validadorCriacao, dto);
            if (resultadoValidacao != null) return resultadoValidacao;

            if (dto.Tipo == TipoIndicador.Nenhum)
                return ResultadoOperacao<IndicadorDto>.CriarFalha("O tipo do indicador é obrigatório.");

            var indicador = _mapper.Map<Indicador>(dto);
            await _repositorioIndicador.AdicionarAsync(indicador);

            return ResultadoOperacao<IndicadorDto>.CriarSucesso(_mapper.Map<IndicadorDto>(indicador));
        }

        public async Task<ResultadoOperacao<IndicadorDto>> AtualizarAsync(AtualizarIndicadorDto dto)
        {
            var resultadoValidacao = await ValidarDtoAsync(_validadorAtualizacao, dto);
            if (resultadoValidacao != null) return resultadoValidacao;

            var existente = await _repositorioIndicador.ObterPorIdAsync(dto.Id);
            if (existente == null)
                return ResultadoOperacao<IndicadorDto>.CriarFalha($"Indicador com ID {dto.Id} não encontrado.");

            _mapper.Map(dto, existente);
            await _repositorioIndicador.AtualizarAsync(existente);

            return ResultadoOperacao<IndicadorDto>.CriarSucesso(_mapper.Map<IndicadorDto>(existente));
        }

        public async Task<ResultadoOperacao> RemoverAsync(int id)
        {
            var indicador = await _repositorioIndicador.ObterPorIdAsync(id);
            if (indicador == null)
                return ResultadoOperacao.CriarFalha($"Indicador com ID {id} não encontrado.");

            await _repositorioIndicador.RemoverAsync(id);
            return ResultadoOperacao.CriarSucesso("Indicador removido com sucesso.");
        }

        public async Task<ResultadoOperacao<bool>> PodeSerRemovidoAsync(int id)
        {
            var indicador = await _repositorioIndicador.ObterPorIdAsync(id);
            if (indicador == null)
                return ResultadoOperacao<bool>.CriarFalha("Indicador não encontrado.");

            if (await _repositorioMeta.ExisteMetaAtivaParaIndicadorAsync(id))
                return ResultadoOperacao<bool>.CriarFalha("Não é possível remover o indicador pois existem metas associadas.");

            return ResultadoOperacao<bool>.CriarSucesso(true, "Pode ser removido.");
        }

        public async Task<ResultadoOperacao> JaExisteComNomeAsync(string nome, int? idExcluir = null)
        {
            var existe = await _repositorioIndicador.JaExisteComNomeAsync(nome, idExcluir);
            return existe
                ? ResultadoOperacao.CriarFalha("Já existe um indicador com esse nome!")
                : ResultadoOperacao.CriarSucesso();
        }

        public Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> ObterComMetasAtivasAsync()
        {
            return Task.FromResult(ResultadoOperacao<IEnumerable<IndicadorDto>>.CriarFalha("Método ainda não implementado."));
        }
    }
}

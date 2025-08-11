using Aplicacao.Utils;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;

namespace Ranking.Aplicacao.Servicos
{
    public class MetaServico : IMetaServico
    {
        private readonly IRepositorioMeta _repositorioMeta;
        private readonly IRepositorioIndicador _repositorioIndicador;
        private readonly IValidator<CriarMetaDto> _validadorCriacao;
        private readonly IValidator<AtualizarMetaDto> _validadorAtualizacao;
        private readonly IMapper _mapper;
        private readonly ILogger<MetaServico> _logger;

        public MetaServico(
            IRepositorioMeta repositorioMeta,
            IRepositorioIndicador repositorioIndicador,
            IValidator<CriarMetaDto> validadorCriacao,
            IValidator<AtualizarMetaDto> validadorAtualizacao,
            IMapper mapper,
            ILogger<MetaServico> logger)
        {
            _repositorioMeta = repositorioMeta;
            _repositorioIndicador = repositorioIndicador;
            _validadorCriacao = validadorCriacao;
            _validadorAtualizacao = validadorAtualizacao;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultadoOperacao<IEnumerable<MetaDto>>> ObterTodosAsync()
        {
            var metas = await _repositorioMeta.ObterTodosAsync();

            var resultado = metas.Select(meta => new MetaDto
            {
                Id = meta.Id,
                IndicadorId = meta.IndicadorId,
                NomeIndicador = meta.Indicador?.Nome ?? "", // nome
                Valor = meta.Valor,
                DataInicio = meta.DataInicio,
                DataFim = meta.DataFim,
                Ativa = meta.Ativa,
                DataCriacao = meta.DataCriacao,
                DataAtualizacao = meta.DataAtualizacao
            });

            return ResultadoOperacao<IEnumerable<MetaDto>>.CriarSucesso(resultado);
        }


        public async Task<ResultadoOperacao<MetaDto>> ObterPorIdAsync(int id)
        {
            var meta = await _repositorioMeta.ObterPorIdAsync(id);
            if (meta == null)
                return ResultadoOperacao<MetaDto>.CriarFalha("Meta não encontrada.");
            return ResultadoOperacao<MetaDto>.CriarSucesso(_mapper.Map<MetaDto>(meta));
        }

        public async Task<ResultadoOperacao<IEnumerable<MetaDto>>> ObterPorIndicadorAsync(int indicadorId)
        {
            var metas = await _repositorioMeta.ObterPorIndicadorAsync(indicadorId);
            var metasDto = _mapper.Map<IEnumerable<MetaDto>>(metas);
            return ResultadoOperacao<IEnumerable<MetaDto>>.CriarSucesso(metasDto);
        }

        public async Task<ResultadoOperacao<IEnumerable<MetaDto>>> ObterMetasAtivasAsync(DateTime? data = null)
        {
            var dataReferencia = data ?? DateTime.Today;
            var metas = await _repositorioMeta.ObterMetasAtivasPorDataAsync(dataReferencia);

            if (!metas.Any())
                return ResultadoOperacao<IEnumerable<MetaDto>>.CriarSucesso(Enumerable.Empty<MetaDto>(), "Nenhuma meta ativa encontrada.");

            var metasDto = _mapper.Map<IEnumerable<MetaDto>>(metas);
            return ResultadoOperacao<IEnumerable<MetaDto>>.CriarSucesso(metasDto);
        }

        public async Task<ResultadoOperacao<MetaDto>> ObterMetaAtivaAtualPorIndicadorAsync(int indicadorId, DateTime? data = null)
        {
            var dataReferencia = data ?? DateTime.Today;
            var meta = await _repositorioMeta.ObterMetaAtivaPorIndicadorEDataAsync(indicadorId, dataReferencia);

            if (meta == null)
                return ResultadoOperacao<MetaDto>.CriarFalha("Nenhuma meta ativa encontrada para o indicador informado.");

            return ResultadoOperacao<MetaDto>.CriarSucesso(_mapper.Map<MetaDto>(meta));
        }

        public async Task<ResultadoOperacao<IEnumerable<MetaDto>>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var metas = await _repositorioMeta.ObterPorPeriodoAsync(dataInicio, dataFim);

            if (metas == null || !metas.Any())
                return ResultadoOperacao<IEnumerable<MetaDto>>.CriarSucesso(Enumerable.Empty<MetaDto>(), "Nenhuma meta encontrada para o período informado.");

            return ResultadoOperacao<IEnumerable<MetaDto>>.CriarSucesso(_mapper.Map<IEnumerable<MetaDto>>(metas));
        }

        public async Task<ResultadoOperacao<MetaDto>> CriarAsync(CriarMetaDto dto)
        {
            var validacao = await _validadorCriacao.ValidateAsync(dto);
            if (!validacao.IsValid)
                return ResultadoOperacao<MetaDto>.CriarFalha(validacao.Errors.Select(e => e.ErrorMessage).ToList());

            var indicador = await _repositorioIndicador.ObterPorIdAsync(dto.IndicadorId);
            if (indicador == null)
                return ResultadoOperacao<MetaDto>.CriarFalha($"Indicador com ID {dto.IndicadorId} não encontrado.");

            var meta = _mapper.Map<Meta>(dto);
            await _repositorioMeta.AdicionarAsync(meta);

            return ResultadoOperacao<MetaDto>.CriarSucesso(_mapper.Map<MetaDto>(meta));
        }

        public async Task<ResultadoOperacao<MetaDto>> AtualizarAsync(AtualizarMetaDto dto)
        {
            var meta = await _repositorioMeta.ObterPorIdAsync(dto.Id);
            if (meta == null)
                return ResultadoOperacao<MetaDto>.CriarFalha("Meta não encontrada.");

            var validacao = await _validadorAtualizacao.ValidateAsync(dto);
            if (!validacao.IsValid)
                return ResultadoOperacao<MetaDto>.CriarFalha(validacao.Errors.Select(e => e.ErrorMessage).ToList());

            // Atualiza entidade usando método da entidade
            meta.Atualizar(dto.IndicadorId, dto.Valor, dto.DataInicio ?? DateTime.Now, dto.DataFim, dto.Ativa);

            await _repositorioMeta.SalvarAlteracoesAsync();

            return ResultadoOperacao<MetaDto>.CriarSucesso(_mapper.Map<MetaDto>(meta), "Meta atualizada com sucesso.");
        }

        public async Task<ResultadoOperacao> RemoverAsync(int id)
        {
            var meta = await _repositorioMeta.ObterPorIdAsync(id);
            if (meta == null)
                return ResultadoOperacao.CriarFalha($"Meta com ID {id} não encontrada.");

            await _repositorioMeta.RemoverAsync(id);
            return ResultadoOperacao.CriarSucesso();
        }

        public async Task<ResultadoOperacao> AtivarAsync(int id)
        {
            var meta = await _repositorioMeta.ObterPorIdAsync(id);
            if (meta == null)
                return ResultadoOperacao.CriarFalha($"Meta com ID {id} não encontrada.");

            meta.Ativar();
            await _repositorioMeta.AtualizarAsync(meta);

            return ResultadoOperacao.CriarSucesso();
        }

        public async Task<ResultadoOperacao> DesativarAsync(int id)
        {
            var meta = await _repositorioMeta.ObterPorIdAsync(id);
            if (meta == null)
                return ResultadoOperacao.CriarFalha($"Meta com ID {id} não encontrada.");

            meta.Desativar();
            await _repositorioMeta.AtualizarAsync(meta);

            return ResultadoOperacao.CriarSucesso();
        }

        public async Task<ResultadoOperacao<int>> DesativarMetasDoIndicadorAsync(int indicadorId)
        {
            var metas = await _repositorioMeta.ObterMetasAtivasPorIndicadorAsync(indicadorId);

            if (metas == null || !metas.Any())
                return ResultadoOperacao<int>.CriarSucesso(0, "Nenhuma meta ativa encontrada para o indicador.");

            foreach (var meta in metas)
            {
                meta.Ativa = false;
            }

            await _repositorioMeta.SalvarAlteracoesAsync();

            return ResultadoOperacao<int>.CriarSucesso(metas.Count(), "Metas desativadas com sucesso.");
        }

        public async Task<ResultadoOperacao<bool>> ExisteMetaAtivaParaIndicadorAsync(int indicadorId, DateTime? data = null, int? idExcluir = null)
        {
            var dataReferencia = data ?? DateTime.Today;

            var existe = await _repositorioMeta.ExisteMetaAtivaParaIndicadorAsync(indicadorId, dataReferencia, idExcluir);

            return ResultadoOperacao<bool>.CriarSucesso(existe);
        }
    }
}

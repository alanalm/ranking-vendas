using Microsoft.EntityFrameworkCore;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;
using Ranking.Infraestrutura.Persistencia;

namespace Ranking.Infraestrutura.Repositorios
{
    public class MetaRepositorio : RepositorioBase<Meta>, IRepositorioMeta
    {
        /// Construtor que recebe o contexto do Entity Framework.
        /// <param name="contexto">Contexto do Entity Framework.</param>
        public MetaRepositorio(RankingVendasContexto contexto) : base(contexto)
        {
        }

        /// Obtém metas por indicador.
        /// <param name="indicadorId">Identificador do indicador.</param>
        /// <returns>Coleção de metas do indicador.</returns>
        public async Task<IEnumerable<Meta>> ObterPorIndicadorAsync(int indicadorId)
        {
            if (indicadorId <= 0)
                return Enumerable.Empty<Meta>();

            return await _dbSet
                .Where(m => m.IndicadorId == indicadorId)
                .OrderByDescending(m => m.DataInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Meta>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet
                .Where(m =>
                    m.Ativa &&
                    m.DataInicio <= dataFim &&
                    (m.DataFim == null || m.DataFim >= dataInicio))
                .ToListAsync();
        }

        public async Task<IEnumerable<Meta>> ObterMetasAtivasPorDataAsync(DateTime data)
        {
            return await _dbSet
                .Where(m => m.Ativa &&
                            m.DataInicio <= data &&
                            (m.DataFim == null || m.DataFim >= data))
                .ToListAsync();
        }

        public async Task<Meta?> ObterMetaAtivaPorIndicadorEDataAsync(int indicadorId, DateTime data)
        {
            return await _dbSet
                .Where(m => m.IndicadorId == indicadorId &&
                            m.Ativa &&
                            m.DataInicio <= data &&
                            (m.DataFim == null || m.DataFim >= data))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtém uma meta com o indicador carregado.
        /// </summary>
        /// <param name="id">Identificador da meta.</param>
        /// <returns>Meta com indicador carregado ou null se não existir.</returns>
        public async Task<Meta?> ObterComIndicadorAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Indicador)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Obtém todas as metas com indicadores carregados.
        /// </summary>
        /// <returns>Coleção de metas com indicadores carregados.</returns>
        public async Task<IEnumerable<Meta>> ObterTodosComIndicadorAsync()
        {
            return await _dbSet
                .Include(m => m.Indicador)
                .OrderBy(m => m.Indicador!.Nome)
                .ThenByDescending(m => m.DataInicio)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém metas ativas.
        /// </summary>
        /// <param name="data">Data para verificação (opcional, padrão é hoje).</param>
        /// <returns>Coleção de metas ativas na data especificada.</returns>
        public async Task<IEnumerable<Meta>> ObterMetasAtivasAsync(DateTime? data = null)
        {
            var dataVerificacao = data ?? DateTime.UtcNow;

            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Indicador)
                .Where(m => m.Ativa &&
                           m.DataInicio <= dataVerificacao &&
                           (m.DataFim == null || m.DataFim >= dataVerificacao))
                .OrderBy(m => m.Indicador!.Nome)
                .ThenByDescending(m => m.DataInicio)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém metas ativas por indicador.
        /// </summary>
        /// <param name="indicadorId">Identificador do indicador.</param>
        /// <param name="data">Data para verificação (opcional, padrão é hoje).</param>
        /// <returns>Coleção de metas ativas do indicador na data especificada.</returns>
        public async Task<IEnumerable<Meta>> ObterMetasAtivasPorIndicadorAsync(int indicadorId, DateTime? data = null)
        {
            if (indicadorId <= 0)
                return Enumerable.Empty<Meta>();

            var dataVerificacao = data ?? DateTime.UtcNow;

            return await _dbSet
                .Include(m => m.Indicador)
                .Where(m => m.IndicadorId == indicadorId &&
                           m.Ativa &&
                           m.DataInicio <= dataVerificacao &&
                           (m.DataFim == null || m.DataFim >= dataVerificacao))
                .OrderByDescending(m => m.DataInicio)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém a meta ativa mais recente para um indicador.
        /// </summary>
        /// <param name="indicadorId">Identificador do indicador.</param>
        /// <param name="data">Data para verificação (opcional, padrão é hoje).</param>
        /// <returns>Meta ativa mais recente ou null se não existir.</returns>
        public async Task<Meta?> ObterMetaAtivaAtualPorIndicadorAsync(int indicadorId, DateTime? data = null)
        {
            if (indicadorId <= 0)
                return null;

            var dataVerificacao = data ?? DateTime.UtcNow;

            return await _dbSet
                .Include(m => m.Indicador)
                .Where(m => m.IndicadorId == indicadorId &&
                           m.Ativa &&
                           m.DataInicio <= dataVerificacao &&
                           (m.DataFim == null || m.DataFim >= dataVerificacao))
                .OrderByDescending(m => m.DataInicio)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtém metas que estão vigentes no período especificado.
        /// </summary>
        /// <param name="dataInicio">Data de início do período.</param>
        /// <param name="dataFim">Data de fim do período.</param>
        /// <returns>Coleção de metas vigentes no período.</returns>
        public async Task<IEnumerable<Meta>> ObterMetasVigentesNoPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            if (dataInicio > dataFim)
                throw new ArgumentException("A data de início deve ser anterior à data de fim.");

            return await _dbSet
                .AsNoTracking()
                .Include(m => m.Indicador)
                .Where(m => m.Ativa &&
                           m.DataInicio <= dataFim &&
                           (m.DataFim == null || m.DataFim >= dataInicio))
                .OrderBy(m => m.Indicador!.Nome)
                .ThenByDescending(m => m.DataInicio)
                .ToListAsync();
        }

        /// <summary>
        /// Desativa todas as metas de um indicador.
        /// </summary>
        /// <param name="indicadorId">Identificador do indicador.</param>
        /// <returns>Número de metas desativadas.</returns>
        public async Task<int> DesativarMetasDoIndicadorAsync(int indicadorId)
        {
            if (indicadorId <= 0)
                return 0;

            var metas = await _dbSet
                .Where(m => m.IndicadorId == indicadorId && m.Ativa)
                .ToListAsync();

            if (!metas.Any())
                return 0;

            foreach (var meta in metas)
            {
                meta.Desativar();
            }

            return metas.Count;
        }

        /// <summary>
        /// Verifica se existe uma meta ativa para o indicador na data especificada.
        /// </summary>
        /// <param name="indicadorId">Identificador do indicador.</param>
        /// <param name="data">Data para verificação (opcional, padrão é hoje).</param>
        /// <param name="idExcluir">ID da meta a ser excluída da verificação (para atualizações).</param>
        /// <returns>True se existe uma meta ativa, false caso contrário.</returns>
        public async Task<bool> ExisteMetaAtivaParaIndicadorAsync(int indicadorId, DateTime? data = null, int? idExcluir = null)
        {
            if (indicadorId <= 0)
                return false;

            var dataVerificacao = data ?? DateTime.UtcNow;
            var query = _dbSet.Where(m => m.IndicadorId == indicadorId &&
                                         m.Ativa == true &&
                                         m.DataInicio <= dataVerificacao &&
                                         (m.DataFim == null || m.DataFim >= dataVerificacao));

            if (idExcluir.HasValue)
                query = query.Where(m => m.Id != idExcluir.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Sobrescrita do método ObterTodosAsync para incluir ordenação.
        /// </summary>
        /// <returns>Coleção de todas as metas ordenadas.</returns>
        public override async Task<IEnumerable<Meta>> ObterTodosAsync()
        {
            return await _dbSet
                .Include(m => m.Indicador)
                .OrderBy(m => m.Indicador!.Nome)
                .ThenByDescending(m => m.DataInicio)
                .ToListAsync();
        }

        public async Task<bool> ExisteMetaAtivaNoPeriodo(DateTime inicio, DateTime fim)
        {
            return await _dbSet
                .AnyAsync(m =>
                    m.Ativa &&
                    m.DataInicio <= fim &&
                    (m.DataFim == null || m.DataFim >= inicio));
        }
    }
}

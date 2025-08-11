using Microsoft.EntityFrameworkCore;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;
using Ranking.Infraestrutura.Persistencia;

namespace Ranking.Infraestrutura.Repositorios
{
    public class IndicadorRepositorio : RepositorioBase<Indicador>, IRepositorioIndicador
    {
        /// Construtor que recebe o contexto do Entity Framework.
        /// <param name="contexto">Contexto do Entity Framework.</param>
        public IndicadorRepositorio(RankingVendasContexto contexto) : base(contexto)
        {
        }

        /// Obtém um indicador por nome.
        /// <param name="nome">Nome do indicador.</param>
        /// <returns>Indicador encontrado ou null se não existir.</returns>
        public async Task<Indicador?> ObterPorNomeAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(i => i.Nome.ToLower() == nome.ToLower().Trim());
        }

        /// Obtém indicadores cujo nome contém o texto especificado.
        /// <param name="textoNome">Texto a ser pesquisado no nome.</param>
        /// <returns>Coleção de indicadores que atendem ao critério.</returns>
        public async Task<IEnumerable<Indicador>> PesquisarPorNomeAsync(string textoNome)
        {
            if (string.IsNullOrWhiteSpace(textoNome))
                return await ObterTodosAsync();

            var textoPesquisa = textoNome.ToLower().Trim();

            return await _dbSet
                .Where(i => i.Nome.ToLower().Contains(textoPesquisa))
                .OrderBy(i => i.Nome)
                .ToListAsync();
        }

        /// Obtém um indicador com suas metas carregadas.
        /// <param name="id">Identificador do indicador.</param>
        /// <returns>Indicador com metas carregadas ou null se não existir.</returns>
        public async Task<Indicador?> ObterComMetasAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Metas.OrderByDescending(m => m.DataInicio))
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        /// Obtém todos os indicadores com suas metas carregadas.
        /// <returns>Coleção de indicadores com metas carregadas.</returns>
        public async Task<IEnumerable<Indicador>> ObterTodosComMetasAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Metas.OrderByDescending(m => m.DataInicio))
                .OrderBy(i => i.Nome)
                .ToListAsync();
        }

        /// Obtém indicadores que possuem metas ativas.
        /// <returns>Coleção de indicadores com metas ativas.</returns>
        public async Task<IEnumerable<Indicador>> ObterComMetasAtivasAsync()
        {
            var dataAtual = DateTime.UtcNow;

            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Metas.Where(m => m.Ativa &&
                                               m.DataInicio <= dataAtual &&
                                               (m.DataFim == null || m.DataFim >= dataAtual)))
                .Where(i => i.Metas.Any(m => m.Ativa &&
                                           m.DataInicio <= dataAtual &&
                                           (m.DataFim == null || m.DataFim >= dataAtual)))
                .OrderBy(i => i.Nome)
                .ToListAsync();
        }

        /// Verifica se já existe um indicador com o nome especificado.
        /// <param name="nome">Nome do indicador.</param>
        /// <param name="idExcluir">ID do indicador a ser excluído da verificação (para atualizações).</param>
        /// <returns>True se já existe um indicador com o nome, false caso contrário.</returns>
        public async Task<bool> JaExisteComNomeAsync(string nome, int? idExcluir = null)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return false;

            var nomeNormalizado = nome.ToLower().Trim();
            var query = _dbSet.Where(i => i.Nome.ToLower() == nomeNormalizado);

            if (idExcluir.HasValue)
                query = query.Where(i => i.Id != idExcluir.Value);

            return await query.AnyAsync();
        }

        /// Sobrescrita do método ObterTodosAsync para incluir ordenação por nome.
        /// <returns>Coleção de todos os indicadores ordenados por nome.</returns>
        public override async Task<IEnumerable<Indicador>> ObterTodosAsync()
        {
            return await _dbSet
                .OrderBy(i => i.Nome)
                .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;
using Ranking.Infraestrutura.Persistencia;
using System.Linq.Expressions;

namespace Ranking.Infraestrutura.Repositorios
{
    public abstract class RepositorioBase<TEntidade> : IRepositorioBase<TEntidade> where TEntidade : EntidadeBase
    {
        /// Contexto do Entity Framework.
        protected readonly RankingVendasContexto _contexto;

        /// DbSet da entidade.
        protected readonly DbSet<TEntidade> _dbSet;

        /// Construtor que recebe o contexto do Entity Framework.
        /// <param name="contexto">Contexto do Entity Framework.</param>
        protected RepositorioBase(RankingVendasContexto contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
            _dbSet = _contexto.Set<TEntidade>();
        }

        /// Obtém uma entidade por seu identificador.
        /// <param name="id">Identificador da entidade.</param>
        /// <returns>Entidade encontrada ou null se não existir.</returns>
        public virtual async Task<TEntidade?> ObterPorIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _dbSet.FindAsync(id);
        }

        /// Obtém todas as entidades.
        /// <returns>Coleção de todas as entidades.</returns>
        public virtual async Task<IEnumerable<TEntidade>> ObterTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// Obtém entidades que atendem ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar as entidades.</param>
        /// <returns>Coleção de entidades que atendem ao critério.</returns>
        public virtual async Task<IEnumerable<TEntidade>> ObterPorCriterioAsync(Expression<Func<TEntidade, bool>> criterio)
        {
            if (criterio == null)
                throw new ArgumentNullException(nameof(criterio));

            return await _dbSet.Where(criterio).ToListAsync();
        }

        /// Obtém a primeira entidade que atende ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar a entidade.</param>
        /// <returns>Primeira entidade que atende ao critério ou null se não existir.</returns>
        public virtual async Task<TEntidade?> ObterPrimeiroPorCriterioAsync(Expression<Func<TEntidade, bool>> criterio)
        {
            if (criterio == null)
                throw new ArgumentNullException(nameof(criterio));

            return await _dbSet.FirstOrDefaultAsync(criterio);
        }

        /// Adiciona uma nova entidade.
        /// <param name="entidade">Entidade a ser adicionada.</param>
        /// <returns>Entidade adicionada com o ID gerado.</returns>
        public virtual async Task<TEntidade> AdicionarAsync(TEntidade entidade)
        {
            if (entidade == null)
                throw new ArgumentNullException(nameof(entidade));

            if (!entidade.EhValida())
                throw new InvalidOperationException("A entidade não é válida.");

            await _dbSet.AddAsync(entidade);
            await _contexto.SaveChangesAsync();
            return entidade;
        }

        /// Adiciona múltiplas entidades.
        /// <param name="entidades">Coleção de entidades a serem adicionadas.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        public virtual async Task AdicionarVariasAsync(IEnumerable<TEntidade> entidades)
        {
            if (entidades == null)
                throw new ArgumentNullException(nameof(entidades));

            var listaEntidades = entidades.ToList();

            if (!listaEntidades.Any())
                return;

            foreach (var entidade in listaEntidades)
            {
                if (!entidade.EhValida())
                    throw new InvalidOperationException($"A entidade {entidade.GetType().Name} com ID {entidade.Id} não é válida.");
            }

            await _dbSet.AddRangeAsync(listaEntidades);
            await _contexto.SaveChangesAsync();
        }

        /// Atualiza uma entidade existente.
        /// <param name="entidade">Entidade a ser atualizada.</param>
        /// <returns>Entidade atualizada.</returns>
        public virtual async Task<TEntidade> AtualizarAsync(TEntidade entidade)
        {
            if (entidade == null)
                throw new ArgumentNullException(nameof(entidade));

            if (!entidade.EhValida())
                throw new InvalidOperationException("A entidade não é válida.");

            _dbSet.Update(entidade);
            await _contexto.SaveChangesAsync();
            return entidade;
        }

        /// Remove uma entidade por seu identificador.
        /// <param name="id">Identificador da entidade a ser removida.</param>
        /// <returns>True se a entidade foi removida, false se não foi encontrada.</returns>
        public virtual async Task<bool> RemoverAsync(int id)
        {
            if (id <= 0)
                return false;

            var entidade = await ObterPorIdAsync(id);
            if (entidade == null)
                return false;

            _dbSet.Remove(entidade);
            await _contexto.SaveChangesAsync();
            return true;
        }

        /// Remove uma entidade.
        /// <param name="entidade">Entidade a ser removida.</param>
        /// <returns>True se a entidade foi removida, false caso contrário.</returns>
        public virtual async Task<bool> RemoverAsync(TEntidade entidade)
        {
            if (entidade == null)
                return false;

            _dbSet.Remove(entidade);
            await _contexto.SaveChangesAsync();
            return true;
        }

        /// Remove múltiplas entidades que atendem ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar as entidades a serem removidas.</param>
        /// <returns>Número de entidades removidas.</returns>
        public virtual async Task<int> RemoverPorCriterioAsync(Expression<Func<TEntidade, bool>> criterio)
        {
            if (criterio == null)
                throw new ArgumentNullException(nameof(criterio));

            var entidades = await _dbSet.Where(criterio).ToListAsync();

            if (!entidades.Any())
                return 0;

            _dbSet.RemoveRange(entidades);
            return entidades.Count;
        }

        /// Verifica se existe alguma entidade que atende ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar as entidades.</param>
        /// <returns>True se existe pelo menos uma entidade que atende ao critério, false caso contrário.</returns>
        public virtual async Task<bool> ExisteAsync(Expression<Func<TEntidade, bool>> criterio)
        {
            if (criterio == null)
                throw new ArgumentNullException(nameof(criterio));

            return await _dbSet.AnyAsync(criterio);
        }

        /// Conta o número de entidades que atendem ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar as entidades (opcional).</param>
        /// <returns>Número de entidades que atendem ao critério.</returns>
        public virtual async Task<int> ContarAsync(Expression<Func<TEntidade, bool>>? criterio = null)
        {
            if (criterio == null)
                return await _dbSet.CountAsync();

            return await _dbSet.CountAsync(criterio);
        }

        /// Obtém entidades com paginação.
        /// <param name="pagina">Número da página (baseado em 1).</param>
        /// <param name="tamanhoPagina">Tamanho da página.</param>
        /// <param name="criterio">Expressão lambda para filtrar as entidades (opcional).</param>
        /// <returns>Coleção paginada de entidades.</returns>
        public virtual async Task<IEnumerable<TEntidade>> ObterPaginadoAsync(int pagina, int tamanhoPagina, Expression<Func<TEntidade, bool>>? criterio = null)
        {
            if (pagina <= 0)
                throw new ArgumentException("O número da página deve ser maior que zero.", nameof(pagina));

            if (tamanhoPagina <= 0)
                throw new ArgumentException("O tamanho da página deve ser maior que zero.", nameof(tamanhoPagina));

            var query = _dbSet.AsQueryable();

            if (criterio != null)
                query = query.Where(criterio);

            return await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();
        }

        /// Salva todas as alterações pendentes no contexto.
        /// <returns>Número de entidades afetadas.</returns>
        public virtual async Task<int> SalvarAlteracoesAsync()
        {
            return await _contexto.SaveChangesAsync();
        }

        /// Libera os recursos utilizados pelo repositório.
        public virtual void Dispose()
        {
            _contexto?.Dispose();
        }
    }
}

using Ranking.Dominio.Entidades;
using System.Linq.Expressions;

namespace Ranking.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioBase<TEntidade> where TEntidade : EntidadeBase
    {
        /// Obtém uma entidade por seu identificador.
        Task<TEntidade?> ObterPorIdAsync(int id);

        /// Obtém todas as entidades.
        /// <returns>Coleção de todas as entidades.</returns>
        Task<IEnumerable<TEntidade>> ObterTodosAsync();

        /// Obtém entidades que atendem ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar as entidades.</param>
        /// <returns>Coleção de entidades que atendem ao critério.</returns>
        Task<IEnumerable<TEntidade>> ObterPorCriterioAsync(Expression<Func<TEntidade, bool>> criterio);

        /// Obtém a primeira entidade que atende ao critério especificado.
        /// <returns>Primeira entidade que atende ao critério ou null se não existir.</returns>
        Task<TEntidade?> ObterPrimeiroPorCriterioAsync(Expression<Func<TEntidade, bool>> criterio);

        /// Adiciona uma nova entidade.
        /// <param name="entidade">Entidade a ser adicionada.</param>
        /// <returns>Entidade adicionada com o ID gerado.</returns>
        Task<TEntidade> AdicionarAsync(TEntidade entidade);

        /// Adiciona múltiplas entidades.
        /// <param name="entidades">Coleção de entidades a serem adicionadas.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        Task AdicionarVariasAsync(IEnumerable<TEntidade> entidades);

        /// Atualiza uma entidade existente.
        /// <param name="entidade">Entidade a ser atualizada.</param>
        Task<TEntidade> AtualizarAsync(TEntidade entidade);

        /// Remove uma entidade por seu identificador.
        /// <returns>True se a entidade foi removida, false se não foi encontrada.</returns>
        Task<bool> RemoverAsync(int id);

        /// Remove uma entidade.
        /// <returns>True se a entidade foi removida, false caso contrário.</returns>
        Task<bool> RemoverAsync(TEntidade entidade);

        /// Remove múltiplas entidades que atendem ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar as entidades a serem removidas.</param>
        /// <returns>Número de entidades removidas.</returns>
        Task<int> RemoverPorCriterioAsync(Expression<Func<TEntidade, bool>> criterio);

        /// Verifica se existe alguma entidade que atende ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar as entidades.</param>
        /// <returns>True se existe pelo menos uma entidade que atende ao critério, false caso contrário.</returns>
        Task<bool> ExisteAsync(Expression<Func<TEntidade, bool>> criterio);

        /// Conta o número de entidades que atendem ao critério especificado.
        /// <param name="criterio">Expressão lambda para filtrar as entidades (opcional).</param>
        /// <returns>Número de entidades que atendem ao critério.</returns>
        Task<int> ContarAsync(Expression<Func<TEntidade, bool>>? criterio = null);

        /// Obtém entidades com paginação.
        /// <param name="pagina">Número da página (baseado em 1).</param>
        /// <param name="tamanhoPagina">Tamanho da página.</param>
        /// <param name="criterio">Expressão lambda para filtrar as entidades (opcional).</param>
        /// <returns>Coleção paginada de entidades.</returns>
        Task<IEnumerable<TEntidade>> ObterPaginadoAsync(int pagina, int tamanhoPagina, Expression<Func<TEntidade, bool>>? criterio = null);

        /// Salva todas as alterações pendentes no contexto.
        /// <returns>Número de entidades afetadas.</returns>
        Task<int> SalvarAlteracoesAsync();
    }
}

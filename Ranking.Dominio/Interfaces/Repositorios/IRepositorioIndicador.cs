using Ranking.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranking.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioIndicador : IRepositorioBase<Indicador>
    {
        /// <returns>Indicador encontrado ou null se não existir.</returns>
        Task<Indicador?> ObterPorNomeAsync(string nome);

        Task<IEnumerable<Indicador>> PesquisarPorNomeAsync(string textoNome);

        /// Obtém um indicador com suas metas carregadas.
        /// <returns>Indicador com metas carregadas ou null se não existir.</returns>
        Task<Indicador?> ObterComMetasAsync(int id);

        /// Obtém todos os indicadores com suas metas carregadas.
        /// <returns>Coleção de indicadores com metas carregadas.</returns>
        Task<IEnumerable<Indicador>> ObterTodosComMetasAsync();

        /// Obtém indicadores que possuem metas ativas.
        /// <returns>Coleção de indicadores com metas ativas.</returns>
        Task<IEnumerable<Indicador>> ObterComMetasAtivasAsync();

        /// <param name="nome">Nome do indicador.</param>
        /// <param name="idExcluir">ID do indicador a ser excluído da verificação (para atualizações).</param>
        /// <returns>True se já existe um indicador com o nome, false caso contrário.</returns>
        Task<bool> JaExisteComNomeAsync(string nome, int? idExcluir = null);
    }
}

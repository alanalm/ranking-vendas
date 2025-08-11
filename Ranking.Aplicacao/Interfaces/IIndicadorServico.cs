using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;

namespace Ranking.Aplicacao.Interfaces
{
    public interface IIndicadorServico
    {
        Task<ResultadoOperacao<IndicadorDto>> CriarAsync(CriarIndicadorDto criarIndicadorDto);

        Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> ObterTodosAsync();

        Task<ResultadoOperacao<IndicadorDto>> ObterPorIdAsync(int id);

        Task<ResultadoOperacao<IndicadorDto>> ObterPorNomeAsync(string nome);

        Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> PesquisarPorNomeAsync(string textoNome);

        Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> ObterComMetasAtivasAsync();

        Task<ResultadoOperacao<IndicadorDto>> AtualizarAsync(AtualizarIndicadorDto atualizarIndicadorDto);

        Task<ResultadoOperacao> RemoverAsync(int id);

        Task<ResultadoOperacao<bool>> PodeSerRemovidoAsync(int id);

        Task<ResultadoOperacao> JaExisteComNomeAsync(string nome, int? idExcluir = null);
    }
}

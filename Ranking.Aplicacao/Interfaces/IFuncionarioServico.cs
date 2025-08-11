using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;

namespace Ranking.Aplicacao.Interfaces
{
    public interface IFuncionarioServico
    {
        Task<ResultadoOperacao<IEnumerable<FuncionarioDto>>> ObterTodosAsync();

        Task<ResultadoOperacao<FuncionarioDto>> ObterPorIdAsync(int id);

        Task<ResultadoOperacao<FuncionarioDto>> ObterPorNomeAsync(string nome);

        Task<ResultadoOperacao<IEnumerable<FuncionarioDto>>> PesquisarPorNomeAsync(string textoNome);

        Task<ResultadoOperacao<IEnumerable<FuncionarioDto>>> ObterComEstatisticasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<ResultadoOperacao<FuncionarioDto>> CriarAsync(CriarFuncionarioDto dto);

        Task<ResultadoOperacao<FuncionarioDto>> AtualizarAsync(AtualizarFuncionarioDto dto);

        Task<ResultadoOperacao<bool>> RemoverAsync(int id);

        Task<ResultadoOperacao<bool>> PodeSerRemovidoAsync(int id);

        Task<ResultadoOperacao<bool>> JaExisteComNomeAsync(string nome, int? idExcluir = null);
    }
}

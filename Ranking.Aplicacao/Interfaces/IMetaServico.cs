using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;

namespace Ranking.Aplicacao.Interfaces
{
    public interface IMetaServico
    {
        Task<ResultadoOperacao<IEnumerable<MetaDto>>> ObterTodosAsync();

        Task<ResultadoOperacao<MetaDto>> ObterPorIdAsync(int id);

        Task<ResultadoOperacao<IEnumerable<MetaDto>>> ObterPorIndicadorAsync(int indicadorId);

        Task<ResultadoOperacao<IEnumerable<MetaDto>>> ObterMetasAtivasAsync(DateTime? data = null);

        Task<ResultadoOperacao<MetaDto>> ObterMetaAtivaAtualPorIndicadorAsync(int indicadorId, DateTime? data = null);

        Task<ResultadoOperacao<IEnumerable<MetaDto>>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);

        Task<ResultadoOperacao<MetaDto>> CriarAsync(CriarMetaDto criarMetaDto);

        Task<ResultadoOperacao<MetaDto>> AtualizarAsync(AtualizarMetaDto atualizarMetaDto);

        Task<ResultadoOperacao> RemoverAsync(int id);

        Task<ResultadoOperacao> AtivarAsync(int id);

        Task<ResultadoOperacao> DesativarAsync(int id);

        Task<ResultadoOperacao<int>> DesativarMetasDoIndicadorAsync(int indicadorId);

        Task<ResultadoOperacao<bool>> ExisteMetaAtivaParaIndicadorAsync(int indicadorId, DateTime? data = null, int? idExcluir = null);
    }
}

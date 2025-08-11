using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;

namespace Ranking.Aplicacao.Interfaces
{
    public interface IRankingServico
    {
        Task<ResultadoOperacao<IEnumerable<RankingDto>>> CalcularRankingAsync(FiltroRankingDto filtro);
        Task<ResultadoOperacao<IEnumerable<RankingDto>>> CalcularRankingPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<ResultadoOperacao<DesempenhoDto?>> ObterDesempenhoFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<ResultadoOperacao<EstatisticasRankingDto>> CalcularEstatisticasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<ResultadoOperacao<IEnumerable<RankingDto>>> ObterFuncionariosQueAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<ResultadoOperacao<IEnumerable<RankingDto>>> ObterFuncionariosQueNaoAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<ResultadoOperacao<bool>> ExistemMetasAtivasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}

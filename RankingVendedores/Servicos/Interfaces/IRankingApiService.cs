using Ranking.Aplicacao.DTOs;

namespace RankingVendedores.Servicos.Interfaces
{
    public interface IRankingApiService
    {
        Task<IEnumerable<RankingDto>> CalcularRankingAsync(FiltroRankingDto? filtro = null);
        Task<IEnumerable<RankingDto>> CalcularRankingPeriodoAtualAsync();
        Task<RankingDto?> ObterDesempenhoFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<EstatisticasRankingDto> CalcularEstatisticasRankingAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<IEnumerable<RankingDto>> ObterFuncionariosQueAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<IEnumerable<RankingDto>> ObterFuncionariosQueNaoAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<bool> VerificarMetasAtivasAsync(DateTime? dataInicio, DateTime? dataFim);
    }
}

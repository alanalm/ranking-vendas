using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;

namespace Ranking.Aplicacao.Interfaces
{
    public interface IVendaServico
    {
        Task<List<VendaDto>> ObterTodosAsync();
        Task<ResultadoOperacao<VendaDto>> ObterPorIdAsync(int id);
        Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorFuncionarioAsync(int funcionarioId);
        Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorFuncionarioEPeriodoAsync(int funcionarioId, DateTime dataInicio, DateTime dataFim);
        Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorMesAsync(int mes, int ano);
        Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterPorAnoAsync(int ano);
        Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterMaioresVendasAsync(int quantidade, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<ResultadoOperacao<IEnumerable<VendaDto>>> ObterVendasRecentesAsync(int? quantidade = null);
        Task<IEnumerable<VendaDto>> PesquisarAsync(string termo, DateTime? incio, DateTime? fim);

        Task<ResultadoOperacao<VendaDto>> CriarAsync(CriarVendaDto criarVendaDto);
        Task<ResultadoOperacao<VendaDto>> AtualizarAsync(AtualizarVendaDto atualizarVendaDto);
        Task<ResultadoOperacao> RemoverAsync(int id);

        Task<ResultadoOperacao<Dictionary<int, (decimal TotalVendas, int QuantidadeVendas)>>> CalcularEstatisticasPorFuncionarioAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<ResultadoOperacao<decimal>> CalcularTotalVendasPorFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<ResultadoOperacao<int>> CalcularQuantidadeVendasPorFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<EstatisticasVendasDto> CalcularEstatisticasAsync(DateTime inicio, DateTime? fim);
        Task<EstatisticasVendasDto> ObterEstatisticasVendasAsync(DateTime? dataInicio, DateTime? dataFim);
    }
}

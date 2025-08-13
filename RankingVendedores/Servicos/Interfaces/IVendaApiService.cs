using Ranking.Aplicacao.DTOs;

namespace RankingVendedores.Servicos.Interfaces
{
    public interface IVendaApiService
    {
        Task<IEnumerable<VendaDto>> ObterVendasAsync();
        Task<VendaDto?> ObterVendaPorIdAsync(int id);
        Task<IEnumerable<VendaDto>> ObterVendasPorFuncionarioAsync(int funcionarioId);
        Task<IEnumerable<VendaDto>> ObterVendasPorPeriodoAsync(DateTime inicio, DateTime fim);
        Task<VendaDto> CriarVendaAsync(CriarVendaDto venda);
        Task<VendaDto> AtualizarVendaAsync(AtualizarVendaDto venda);
        Task<List<VendaDto>> PesquisarVendasAsync(string nomeFuncionario, DateTime? dataInicio, DateTime? dataFim);
        Task RemoverVendaAsync(int id);
        Task<EstatisticasVendasDto> ObterEstatisticasVendasAsync(DateTime? dataInicio, DateTime? dataFim);
    }
}

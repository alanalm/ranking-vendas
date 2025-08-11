using Ranking.Dominio.Entidades;
using System.Linq;

namespace Ranking.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioVenda : IRepositorioBase<Venda>
    {
        IQueryable<Venda> ObterQuery();
        Task<List<Venda>> ObterTodos();

        Task<IEnumerable<Venda>> ObterPorFuncionarioAsync(int funcionarioId);

        Task<Venda?> ObterComFuncionarioAsync(int id);

        Task<IEnumerable<Venda>> ObterTodosComFuncionarioAsync();

       Task<IEnumerable<Venda>> ObterPorPeriodoAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<IEnumerable<Venda>> ObterPorFuncionarioEPeriodoAsync(int funcionarioId, DateTime dataInicio, DateTime dataFim);

        Task<IEnumerable<Venda>> ObterPorMesAsync(int mes, int ano);

        Task<IEnumerable<Venda>> ObterPorAnoAsync(int ano);

        Task<IEnumerable<Venda>> PesquisarAsync(string texto);

        Task<decimal> CalcularTotalVendasPorFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<int> CalcularQuantidadeVendasPorFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<Dictionary<int, (decimal TotalVendas, int QuantidadeVendas)>> ObterEstatisticasPorFuncionarioAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<IEnumerable<Venda>> ObterMaioresVendasAsync(int quantidade, DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<IEnumerable<Venda>> ObterVendasRecentesAsync(int? quantidade = null);

        Task<List<Venda>> ObterVendasPorPeriodoAsync(DateTime? dataInicio, DateTime? dataFim);

        Task<bool> TemVendasAsync(int funcionarioId);
    }
}

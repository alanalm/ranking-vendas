using FluentValidation;
using Ranking.Dominio.Entidades;
using Ranking.Dominio.Interfaces.Repositorios;

namespace Ranking.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioFuncionario : IRepositorioBase<Funcionario>
    {
        Task<List<Funcionario>> ObterTodos();
        Task<Funcionario?> ObterPorNomeAsync(string nome);

        Task<IEnumerable<Funcionario>> PesquisarPorNomeAsync(string textoNome);

        Task<Funcionario?> ObterComVendasAsync(int id);

        Task<IEnumerable<Funcionario>> ObterTodosComVendasAsync();

        Task<IEnumerable<Funcionario>> ObterComVendasNoPeriodoAsync(DateTime dataInicio, DateTime dataFim);

        Task<IEnumerable<Funcionario>> ObterRankingPorTotalVendasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<IEnumerable<Funcionario>> ObterRankingPorQuantidadeVendasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);

        Task<bool> JaExisteComNomeAsync(string nome, int? idExcluir = null);
    }
}

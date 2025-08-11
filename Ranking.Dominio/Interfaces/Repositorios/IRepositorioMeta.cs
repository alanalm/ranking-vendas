using Ranking.Dominio.Entidades;

namespace Ranking.Dominio.Interfaces.Repositorios
{
    public interface IRepositorioMeta : IRepositorioBase<Meta>
    {
        Task<IEnumerable<Meta>> ObterPorIndicadorAsync(int indicadorId);

        Task<Meta?> ObterComIndicadorAsync(int id);

        Task<IEnumerable<Meta>> ObterTodosComIndicadorAsync();

        Task<IEnumerable<Meta>> ObterMetasAtivasAsync(DateTime? data = null);

        Task<IEnumerable<Meta>> ObterMetasAtivasPorIndicadorAsync(int indicadorId, DateTime? data = null);

        Task<Meta?> ObterMetaAtivaAtualPorIndicadorAsync(int indicadorId, DateTime? data = null);

        Task<IEnumerable<Meta>> ObterMetasVigentesNoPeriodoAsync(DateTime dataInicio, DateTime dataFim);

        Task<int> DesativarMetasDoIndicadorAsync(int indicadorId);

        Task<bool> ExisteMetaAtivaParaIndicadorAsync(int indicadorId, DateTime? data = null, int? idExcluir = null);

        Task<IEnumerable<Meta>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);

        Task<IEnumerable<Meta>> ObterMetasAtivasPorDataAsync(DateTime data);

        Task<Meta?> ObterMetaAtivaPorIndicadorEDataAsync(int indicadorId, DateTime data);

        Task<bool> ExisteMetaAtivaNoPeriodo(DateTime inicio, DateTime fim);
        //Task ObterMetasAtivasAsync();
    }
}

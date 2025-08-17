using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;

namespace RankingVendedores.Servicos.Interfaces
{
    public interface IIndicadorApiService
    {
        Task<ResultadoOperacao<List<IndicadorDto>>> ObterIndicadoresAsync();
        Task<ResultadoOperacao<IndicadorDto>> ObterIndicadorPorIdAsync(int id);
        Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> PesquisarIndicadoresAsync(string nome);
        Task<ResultadoOperacao<IndicadorDto>> CriarIndicadorAsync(CriarIndicadorDto indicador);
        Task<ResultadoOperacao<IndicadorDto>> AtualizarIndicadorAsync(AtualizarIndicadorDto indicador);
        Task<ResultadoOperacao<bool>> RemoverIndicadorAsync(int id);
        Task<ResultadoOperacao<bool>> PodeIndicadorSerRemovidoAsync(int id);
    }
}

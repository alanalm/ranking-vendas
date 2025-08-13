using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;

namespace RankingVendedores.Servicos.Interfaces
{
    public interface IMetaApiService
    {
        Task<List<MetaDto>> ObterMetasAsync();
        Task<ResultadoOperacao<MetaDto>> ObterMetaPorIdAsync(int id);
        Task<ResultadoOperacao<List<MetaDto>>> ObterMetasComResultadoAsync();
        Task<ResultadoOperacao<List<MetaDto>>> ObterMetasPorIndicadorAsync(int indicadorId);
        Task<ResultadoOperacao<MetaDto>> CriarMetaAsync(CriarMetaDto meta);
        Task<ResultadoOperacao<MetaDto>> AtualizarMetaAsync(AtualizarMetaDto meta);
        Task RemoverMetaAsync(int id);
        Task<ResultadoOperacao<bool>> AtivarMetaAsync(int id);
        Task<ResultadoOperacao<bool>> DesativarMetaAsync(int id);

    }
}

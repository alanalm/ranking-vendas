using Ranking.Aplicacao.DTOs;

namespace Ranking.Aplicacao.Interfaces
{
    public interface IConfiguracoesService
    {
        Task<ConfiguracoesDto> ObterAsync();
        Task SalvarAsync(ConfiguracoesDto configuracoes);
        Task<int> ObterMetaMinimaAsync();
        void AtualizarConfiguracoes(ConfiguracoesDto novas);
    }
}

using Ranking.Aplicacao.DTOs;

namespace Ranking.Aplicacao.Interfaces
{
    public interface IConfiguracoesService
    {
        Task<ConfiguracoesDto> ObterAsync();
        Task SalvarAsync(ConfiguracoesDto configuracoes);

        void AtualizarConfiguracoes(ConfiguracoesDto novas);
    }
}

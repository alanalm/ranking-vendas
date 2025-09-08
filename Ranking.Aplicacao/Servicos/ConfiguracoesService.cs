using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Enums;

namespace Ranking.Aplicacao.Servicos
{
    public class ConfiguracoesService : IConfiguracoesService
    {
        public ConfiguracoesDto Configuracoes { get; private set; }

        public void AtualizarConfiguracoes(ConfiguracoesDto novas)
        {
            Configuracoes = novas;
        }

        public ConfiguracoesService()
        {
            // Inicializa com valores padrão
            Configuracoes = new ConfiguracoesDto
            {
                PeriodoPadrao = PeriodoPadrao.Mensal,
                MetaMinima = 100,
                TopFuncionarios = 10,
            };
        }

        public async Task<ConfiguracoesDto> ObterAsync()
        {
            return await Task.FromResult(Configuracoes);
        }

        public Task<int> ObterMetaMinimaAsync()
        {
            return Task.FromResult(Configuracoes.MetaMinima);
        }

        public Task SalvarAsync(ConfiguracoesDto configuracoes)
        {
            Configuracoes = configuracoes;
            return Task.CompletedTask;
        }
    }
}

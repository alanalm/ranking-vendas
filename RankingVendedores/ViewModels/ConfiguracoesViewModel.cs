using MudBlazor;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;

namespace RankingVendedores.ViewModels
{
    public class ConfiguracoesViewModel
    {
        public MudForm FormGerais { get; set; }
        public MudForm FormRanking { get; set; }

        public ConfiguracoesDto Configuracoes { get; set; } = new();

        private readonly IConfiguracoesService _service;
        private readonly ISnackbar _snackbar;

        public ConfiguracoesViewModel(IConfiguracoesService service, ISnackbar snackbar)
        {
            _service = service;
            _snackbar = snackbar;
        }

        public async Task CarregarAsync()
        {
            Configuracoes = await _service.ObterAsync() ?? new ConfiguracoesDto();
        }

        public async Task SalvarGerais()
        {
            await _service.SalvarAsync(Configuracoes);
            _snackbar.Add("Configurações salvas com sucesso!", Severity.Success);
        }
    }
}

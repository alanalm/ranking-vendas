using MudBlazor;
using Ranking.Aplicacao.DTOs;
using Ranking.Dominio.Enums;
using RankingVendedores.Services;
using System.Collections.ObjectModel;

namespace RankingVendedores.ViewModels
{
    public class IndicadorViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;

        public ObservableCollection<IndicadorDto> Indicadores { get; private set; } = new();
        public ObservableCollection<IndicadorDto> IndicadoresFiltrados { get; private set; } = new();

        private IndicadorDto? _indicadorSelecionado;
        public IndicadorDto? IndicadorSelecionado
        {
            get => _indicadorSelecionado;
            set => SetProperty(ref _indicadorSelecionado, value);
        }

        private string _textoPesquisa = string.Empty;
        public string TextoPesquisa
        {
            get => _textoPesquisa;
            set
            {
                if (SetProperty(ref _textoPesquisa, value))
                    FiltrarIndicadores();
            }
        }

        public IndicadorViewModel(IApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task CarregarIndicadoresAsync()
        {
            var resultado = await _apiService.ObterIndicadoresAsync();

            if (resultado.Sucesso && resultado.Dados is not null)
            {
                Indicadores = new ObservableCollection<IndicadorDto>(resultado.Dados);
                IndicadoresFiltrados = new ObservableCollection<IndicadorDto>(resultado.Dados);
            }
        }


        private void FiltrarIndicadores()
        {
            if (string.IsNullOrWhiteSpace(TextoPesquisa))
            {
                IndicadoresFiltrados = new ObservableCollection<IndicadorDto>(Indicadores);
            }
            else
            {
                var filtrados = Indicadores.Where(i => i.Nome.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase));
                IndicadoresFiltrados = new ObservableCollection<IndicadorDto>(filtrados);
            }

            OnPropertyChanged(nameof(IndicadoresFiltrados));
        }

        public async Task<bool> CriarIndicadorAsync(CriarIndicadorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                DefinirErro("O nome do indicador é obrigatório.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Descricao) || dto.Descricao.Length < 10 || dto.Descricao.Length > 100)
            {
                DefinirErro("A descrição deve ter entre 10 e 100 caracteres.");
                return false;
            }

            if (dto.Tipo == TipoIndicador.Nenhum)
            {
                DefinirErro("O tipo do indicador é obrigatório.");
                return false;
            }

            var resultado = await ExecutarOperacaoAsync(
            () => _apiService.CriarIndicadorAsync(dto),
            "Indicador criado com sucesso!");

            if (resultado.Sucesso)
                await CarregarIndicadoresAsync();

            return resultado.Sucesso;
        }

        public async Task<bool> AtualizarIndicadorAsync(AtualizarIndicadorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                DefinirErro("O nome do indicador é obrigatório.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Descricao) || dto.Descricao.Length < 10 || dto.Descricao.Length > 100)
            {
                DefinirErro("A descrição deve ter entre 10 e 100 caracteres.");
                return false;
            }

            if (dto.Tipo == TipoIndicador.Nenhum)
            {
                DefinirErro("O tipo do indicador é obrigatório.");
                return false;
            }

            var sucesso = await ExecutarOperacaoAsync(async () =>
            {
                await _apiService.AtualizarIndicadorAsync(dto);
                return true;
            }, "Indicador atualizado com sucesso!");

            if (sucesso)
                await CarregarIndicadoresAsync();

            return sucesso;
        }

        public async Task<bool> RemoverIndicadorAsync(IndicadorDto indicador)
        {
            if (indicador == null)
            {
                DefinirErro("Nenhum indicador selecionado para remoção.");
                return false;
            }

            var podeRemover = await _apiService.PodeIndicadorSerRemovidoAsync(indicador.Id);

            if (!podeRemover.Sucesso)
            {
                DefinirErro("Este indicador não pode ser removido pois possui metas associadas.");
                return false;
            }

            var resultado = await ExecutarOperacaoAsync(
       () => _apiService.RemoverIndicadorAsync(indicador.Id),
       "Indicador removido com sucesso!"
   );

            if (resultado?.Sucesso == true)
                await CarregarIndicadoresAsync();

            return resultado?.Sucesso ?? false;
        }
    }
}

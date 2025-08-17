using Aplicacao.Utils;
using MudBlazor;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Validacoes;
using RankingVendedores.Servicos.Interfaces;
using System.Collections.ObjectModel;

namespace RankingVendedores.ViewModels
{
    public class IndicadorViewModel : ViewModelBase
    {
        private readonly IIndicadorApiService _apiService;
        private readonly ValidadorCriarIndicador _validadorCriarIndicador = new();
        private readonly ValidadorAtualizarIndicador _validadorAtualizarIndicador = new();

        public ObservableCollection<IndicadorDto> Indicadores { get; private set; } = new();
        public ObservableCollection<IndicadorDto> IndicadoresFiltrados { get; private set; } = new();
        public ObservableCollection<string> MensagensErro { get; } = new();

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

        public IndicadorViewModel(IIndicadorApiService apiService)
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

        public async Task<ResultadoOperacao> CriarIndicadorAsync(CriarIndicadorDto dto)
        {
            var resultadoValidacao = _validadorCriarIndicador.Validate(dto);
            if (!ValidarDto(_validadorCriarIndicador, dto))
                return ResultadoOperacao<IndicadorDto>.CriarFalha("Dados inválidos");


            var resultado = await ExecutarOperacaoAsync(
                async () =>
                {
                   var indicadorCriado = await _apiService.CriarIndicadorAsync(dto);
                    return indicadorCriado;
                }, "Indicador criado com sucesso!"
            );

            if (resultado.Sucesso)
                await CarregarIndicadoresAsync();

            return resultado;
        }

        public async Task<ResultadoOperacao> AtualizarIndicadorAsync(AtualizarIndicadorDto dto)
        {
            var resultadoValidacao = _validadorAtualizarIndicador.Validate(dto);
            if (!ValidarDto(_validadorAtualizarIndicador, dto))
                return ResultadoOperacao<IndicadorDto>.CriarFalha("Dados inválidos");

            var resultado = await ExecutarOperacaoAsync(
                async () =>
                {
                  var indicadorAtualizado =  await _apiService.AtualizarIndicadorAsync(dto);
                    return indicadorAtualizado;
                }, "Indicador atualizado com sucesso!"
            );

            if (resultado.Sucesso)
                await CarregarIndicadoresAsync();

            return resultado;
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

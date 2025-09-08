using MudBlazor;
using MudBlazor.Charts;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Dominio.Enums;
using RankingVendedores.Servicos.Interfaces;
using System.Collections.ObjectModel;

namespace RankingVendedores.ViewModels
{
    public class RankingViewModel : ViewModelBase
    {
        private readonly IRankingApiService _apiService;
        private readonly IConfiguracoesService _configuracoesService;

        public ObservableCollection<RankingDto> Ranking { get; } = new();

        private EstatisticasRankingDto? _estatisticas;
        public EstatisticasRankingDto? Estatisticas
        {
            get => _estatisticas;
            set => SetProperty(ref _estatisticas, value);
        }

        private DateTime? _dataInicio;
        public DateTime? DataInicio
        {
            get => _dataInicio;
            set => SetProperty(ref _dataInicio, value);
        }

        private DateTime? _dataFim;
        public DateTime? DataFim
        {
            get => _dataFim;
            set => SetProperty(ref _dataFim, value);
        }

        private DateRange? _periodoFiltro;
        public DateRange? PeriodoFiltro
        {
            get => _periodoFiltro;
            set
            {
                if (SetProperty(ref _periodoFiltro, value))
                {
                    if (value is not null)
                    {
                        DataInicio = value.Start;
                        DataFim = value.End;
                        _ = CarregarRankingAsync();
                    }
                }
            }
        }

        private TipoOrdenacaoRanking _tipoOrdenacao = TipoOrdenacaoRanking.DesempenhoGeral;
        public TipoOrdenacaoRanking TipoOrdenacao
        {
            get => _tipoOrdenacao;
            set => SetProperty(ref _tipoOrdenacao, value);
        }

        private bool _mostrarApenasQueAtingiramMetas;
        public bool MostrarApenasQueAtingiramMetas
        {
            get => _mostrarApenasQueAtingiramMetas;
            set => SetProperty(ref _mostrarApenasQueAtingiramMetas, value);
        }

        private bool _mostrarApenasQueNaoAtingiramMetas;
        public bool MostrarApenasQueNaoAtingiramMetas
        {
            get => _mostrarApenasQueNaoAtingiramMetas;
            set => SetProperty(ref _mostrarApenasQueNaoAtingiramMetas, value);
        }

        private RankingDto? _funcionarioSelecionado;
        public RankingDto? FuncionarioSelecionado
        {
            get => _funcionarioSelecionado;
            set => SetProperty(ref _funcionarioSelecionado, value);
        }

        private bool _existemMetasAtivas = true;
        public bool ExistemMetasAtivas
        {
            get => _existemMetasAtivas;
            set => SetProperty(ref _existemMetasAtivas, value);
        }

        private bool _exibindoPeriodoAtual;
        public bool ExibindoPeriodoAtual
        {
            get => _exibindoPeriodoAtual;
            set => SetProperty(ref _exibindoPeriodoAtual, value);
        }

        private int _topFuncionarios = 10;
        public int TopFuncionarios
        {
            get => _topFuncionarios;
            set => SetProperty(ref _topFuncionarios, value);
        }

        private int? _metaMinima;
        public int? MetaMinima
        {
            get => _metaMinima;
            set => SetProperty(ref _metaMinima, value);
        }

        public RankingViewModel(IRankingApiService apiService, IConfiguracoesService configuracoesService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _configuracoesService = configuracoesService ?? throw new ArgumentNullException(nameof(configuracoesService));
        }

        // Inicialização assíncrona obrigatória
        public async Task InicializarAsync()
        {
            await AplicarConfiguracoesGlobaisAsync();
            await CarregarRankingHomeAsync();
            await CarregarEstatisticasAsync();
        }

        private async Task AplicarConfiguracoesGlobaisAsync()
        {
            var cfg = await _configuracoesService.ObterAsync();
            MetaMinima = cfg.MetaMinima;
            TopFuncionarios = cfg.TopFuncionarios;

            var hoje = DateTime.Today;

            switch (cfg.PeriodoPadrao)
            {
                case PeriodoPadrao.Mensal:
                    DataInicio = new DateTime(hoje.Year, hoje.Month, 1);
                    DataFim = DataInicio.Value.AddMonths(1).AddDays(-1);
                    break;

                case PeriodoPadrao.Trimestral:
                    var trimestreInicio = new DateTime(hoje.Year, ((hoje.Month - 1) / 3) * 3 + 1, 1);
                    DataInicio = trimestreInicio;
                    DataFim = trimestreInicio.AddMonths(3).AddDays(-1);
                    break;

                case PeriodoPadrao.Anual:
                    DataInicio = new DateTime(hoje.Year, 1, 1);
                    DataFim = new DateTime(hoje.Year, 12, 31);
                    break;
            }

            PeriodoFiltro = new DateRange(DataInicio.Value, DataFim.Value);
        }

        public async Task CarregarRankingAsync()
        {
            bool metasAtivas;

            try
            {
                metasAtivas = await _apiService.VerificarMetasAtivasAsync(DataInicio, DataFim);
            }
            catch (Exception ex)
            {
                metasAtivas = false;
                DefinirErro("Erro ao verificar metas ativas: " + ex.Message);
                Console.WriteLine(ex);
            }

            ExistemMetasAtivas = metasAtivas;

            if (!ExistemMetasAtivas)
            {
                DefinirErro("Não existem metas ativas no período. Serão exibidos apenas os dados disponíveis.");
            }

            IEnumerable<RankingDto>? ranking = null;

            if (MostrarApenasQueAtingiramMetas)
            {
                ranking = await ExecutarOperacaoAsync(async () =>
                {
                    return await _apiService.ObterFuncionariosQueAtingiramMetasAsync(DataInicio, DataFim);
                });
            }
            else if (MostrarApenasQueNaoAtingiramMetas)
            {
                ranking = await ExecutarOperacaoAsync(async () =>
                {
                    return await _apiService.ObterFuncionariosQueNaoAtingiramMetasAsync(DataInicio, DataFim);
                });
            }
            else
            {
                var filtro = new FiltroRankingDto(DataInicio, DataFim, TipoOrdenacao);
                ranking = await ExecutarOperacaoAsync(async () =>
                {
                    return await _apiService.CalcularRankingAsync(filtro);
                });
            }

            if (ranking != null)
            {
                Ranking.Clear();
                foreach (var item in ranking.OrderBy(r => r.Posicao))
                {
                    Ranking.Add(item);
                }

                await CarregarEstatisticasAsync();
            }
        }

        public async Task CarregarRankingPeriodoAtualAsync()
        {
            EstaCarregando = true;
            try
            {
                // garante que DataInicio/DataFim/MetaMinima estejam corretos
                await AplicarConfiguracoesGlobaisAsync();
                ExibindoPeriodoAtual = true;

                var cfg = await _configuracoesService.ObterAsync();
                var filtro = new FiltroRankingDto
                {
                    DataInicio = this.DataInicio,
                    DataFim = this.DataFim,
                    TipoOrdenacao = TipoOrdenacao,
                    MetaMinima = cfg.MetaMinima,
                    Top = 0
                };

                var rankingCompleto = await _apiService.CalcularRankingAsync(filtro);

                IEnumerable<RankingDto> rankingFiltrado = rankingCompleto;
                if (ExistemMetasAtivas)
                {
                    if (MostrarApenasQueAtingiramMetas)
                        rankingFiltrado = rankingFiltrado.Where(r => r.Desempenho.AtingiuTodasAsMetas);
                    else if (MostrarApenasQueNaoAtingiramMetas)
                        rankingFiltrado = rankingFiltrado.Where(r => !r.Desempenho.AtingiuTodasAsMetas);
                }

                rankingFiltrado = rankingFiltrado.OrderBy(r => r.Posicao);

                if (TopFuncionarios > 0)
                    rankingFiltrado = rankingFiltrado.Take(TopFuncionarios);

                Ranking.Clear();
                foreach (var item in rankingFiltrado)
                    Ranking.Add(item);

                await CarregarEstatisticasAsync();
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        public async Task CarregarRankingHomeAsync()
        {
            var cfg = await _configuracoesService.ObterAsync();
            var filtro = new FiltroRankingDto
            {
                DataInicio = this.DataInicio,
                DataFim = this.DataFim,
                TipoOrdenacao = TipoOrdenacaoRanking.DesempenhoGeral,
                Top = 0,
                MetaMinima = MetaMinima ?? cfg.MetaMinima
            };

            var rankingCompleto = await _apiService.CalcularRankingAsync(filtro);
            Ranking.Clear();
            foreach (var r in rankingCompleto.Take(5))
                Ranking.Add(r);

            await CarregarEstatisticasAsync();
        }

        public async Task CarregarEstatisticasAsync()
        {
            var estatisticas = await ExecutarOperacaoAsync(() =>
                _apiService.CalcularEstatisticasRankingAsync(DataInicio, DataFim)
            );

            if (estatisticas != null)
                Estatisticas = estatisticas;
        }

        public async Task AplicarFiltrosAsync()
        {
            if (DataInicio.HasValue && DataFim.HasValue && DataInicio.Value > DataFim.Value)
            {
                DefinirErro("A data de início deve ser anterior à data de fim.");
                return;
            }

            ExibindoPeriodoAtual = false;
            await CarregarRankingAsync();
        }

        public async Task LimparFiltrosAsync()
        {
            await AplicarConfiguracoesGlobaisAsync();
            await CarregarRankingPeriodoAtualAsync();

            MostrarApenasQueAtingiramMetas = false;
            MostrarApenasQueNaoAtingiramMetas = false;
        }

        public void SelecionarFuncionario(RankingDto funcionario)
        {
            FuncionarioSelecionado = funcionario;
        }

        public void LimparSelecao()
        {
            FuncionarioSelecionado = null;
        }

        public async Task AlternarFiltroAtingiramMetasAsync()
        {
            MostrarApenasQueAtingiramMetas = !MostrarApenasQueAtingiramMetas;
            if (MostrarApenasQueAtingiramMetas)
                MostrarApenasQueNaoAtingiramMetas = false;

            await CarregarRankingAsync();
        }

        public async Task AlternarFiltroNaoAtingiramMetasAsync()
        {
            MostrarApenasQueNaoAtingiramMetas = !MostrarApenasQueNaoAtingiramMetas;
            if (MostrarApenasQueNaoAtingiramMetas)
                MostrarApenasQueAtingiramMetas = false;

            await CarregarRankingAsync();
        }

        public string ObterDescricaoPeriodo()
        {
            if (DataInicio.HasValue && DataFim.HasValue)
            {
                return ExibindoPeriodoAtual
                    ? $"Período Atual: {DataInicio.Value:dd/MM/yyyy} a {DataFim.Value:dd/MM/yyyy}"
                    : $"Período: {DataInicio.Value:dd/MM/yyyy} a {DataFim.Value:dd/MM/yyyy}";
            }

            return "Todos os períodos";
        }

        public async Task AtualizarHomeAsync()
        {
            await CarregarRankingHomeAsync();
        }
    }
}

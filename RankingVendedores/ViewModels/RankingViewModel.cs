using MudBlazor;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Interfaces;
using Ranking.Aplicacao.Servicos;
using Ranking.Dominio.Enums;
using RankingVendedores.Servicos.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        private DateRange? _periodoFiltro = new DateRange(DateTime.Today.AddDays(-30), DateTime.Today);
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
        public RankingViewModel(IRankingApiService apiService, IConfiguracoesService configuracoesService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _configuracoesService = configuracoesService ?? throw new ArgumentNullException(nameof(configuracoesService));

            AplicarConfiguracoesGlobais();
        }

        private async Task ConfiguracoesAlteradas()
        {
           await AplicarConfiguracoesGlobais();
            _ = CarregarRankingAsync();
        }

        private async Task AplicarConfiguracoesGlobais()
        {
            var cfg = await _configuracoesService.ObterAsync();
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
        }


        /// <param name="apiService">Serviço para comunicação com a API.</param>
        //public RankingViewModel(IRankingApiService apiService)
        //{
        //    _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

        //    // Inicializar com período atual (mês atual)
        //    //var hoje = DateTime.Today;
        //    //DataInicio = new DateTime(hoje.Year, hoje.Month, 1);
        //    //DataFim = DataInicio.Value.AddMonths(1).AddDays(-1);

        //    //Ajustando o período para os últimos dois meses para testes.
        //    var hoje = DateTime.Today;
        //    var doisMesesAtras = hoje.AddMonths(-2);
        //    DataInicio = new DateTime(doisMesesAtras.Year, doisMesesAtras.Month, 1);
        //    DataFim = hoje;
        //}

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

                // Carregar estatísticas
                await CarregarEstatisticasAsync();
            }
        }

        public async Task CarregarRankingPeriodoAtualAsync()
        {
            EstaCarregando = true;

            try
            {
               await AplicarConfiguracoesGlobais();

                var filtro = new FiltroRankingDto
                {
                    DataInicio = this.DataInicio,
                    DataFim = this.DataFim,
                    TipoOrdenacao = TipoOrdenacao
                };

                var ranking = await _apiService.CalcularRankingAsync(filtro);

                //var top = _configuracoesService.Configuracoes.TopFuncionarios;
                //if (top > 0)
                //    ranking = ranking.Take(top);

                Ranking.Clear();
                foreach (var item in ranking)
                    Ranking.Add(item);

                await CarregarEstatisticasAsync();
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        public async Task CarregarEstatisticasAsync()
        {
            var estatisticas = await ExecutarOperacaoAsync(async () =>
            {
                DateTime? dataInicio = PeriodoFiltro?.Start;
                DateTime? dataFim = PeriodoFiltro?.End;
                return await _apiService.CalcularEstatisticasRankingAsync(dataInicio, dataFim);
            });

            if (estatisticas != null)
            {
                Estatisticas = estatisticas;
            }
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
            var hoje = DateTime.Today;
            DataInicio = new DateTime(hoje.Year, hoje.Month, 1);
            DataFim = DataInicio.Value.AddMonths(1).AddDays(-1);
            TipoOrdenacao = TipoOrdenacaoRanking.DesempenhoGeral;
            MostrarApenasQueAtingiramMetas = false;
            MostrarApenasQueNaoAtingiramMetas = false;

            await CarregarRankingPeriodoAtualAsync();
        }

        public void SelecionarFuncionario(RankingDto funcionario)
        {
            FuncionarioSelecionado = funcionario;
        }

        public void LimparSelecao()
        {
            FuncionarioSelecionado = null;
        }

        public async Task<RankingDto?> ObterDesempenhoFuncionarioAsync(int funcionarioId)
        {
            return await ExecutarOperacaoAsync(async () =>
            {
                return await _apiService.ObterDesempenhoFuncionarioAsync(funcionarioId, DataInicio, DataFim);
            });
        }

        public async Task AlternarFiltroAtingiramMetasAsync()
        {
            MostrarApenasQueAtingiramMetas = !MostrarApenasQueAtingiramMetas;
            if (MostrarApenasQueAtingiramMetas)
            {
                MostrarApenasQueNaoAtingiramMetas = false;
            }
            await CarregarRankingAsync();
        }

        public async Task AlternarFiltroNaoAtingiramMetasAsync()
        {
            MostrarApenasQueNaoAtingiramMetas = !MostrarApenasQueNaoAtingiramMetas;
            if (MostrarApenasQueNaoAtingiramMetas)
            {
                MostrarApenasQueAtingiramMetas = false;
            }
            await CarregarRankingAsync();
        }

        public string ObterDescricaoPeriodo()
        {
            if (DataInicio.HasValue && DataFim.HasValue)
            {
                if (ExibindoPeriodoAtual)
                {
                    return $"Período Atual: {DataInicio.Value:dd/MM/yyyy} a {DataFim.Value:dd/MM/yyyy}";
                }
                else
                {
                    return $"Período: {DataInicio.Value:dd/MM/yyyy} a {DataFim.Value:dd/MM/yyyy}";
                }
            }
            return "Todos os períodos";
        }
    }
}

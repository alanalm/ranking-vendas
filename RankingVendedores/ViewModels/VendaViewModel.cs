using Aplicacao.Utils;
using MudBlazor;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Validacoes;
using RankingVendedores.Servicos.Interfaces;
using System.Collections.ObjectModel;

namespace RankingVendedores.ViewModels
{
    /// <summary>
    /// ViewModel para gerenciamento de vendas.
    /// Implementa o padrão MVVM com operações CRUD e binding de dados.
    /// </summary>
    public class VendaViewModel : ViewModelBase
    {
        private readonly IVendaApiService _apiService;
        private readonly IFuncionarioApiService _funcionarioApiService;
        private readonly ValidadorCriarVenda _validadorCriarVenda = new();
        private readonly ValidadorAtualizarVenda _validadorAtualizarVenda = new();

        private List<VendaDto> _vendasOriginais = new();

        /// <summary>
        /// Coleção de funcionários disponíveis para seleção.
        /// </summary>
        public ObservableCollection<FuncionarioDto> FuncionariosDisponiveis { get; } = new();

        private List<VendaDto> _vendasFiltradas = new();
        public List<VendaDto> VendasFiltradas
        {
            get => _vendasFiltradas;
            set
            {
                _vendasFiltradas = value;
                OnPropertyChanged(); // se implementa INotifyPropertyChanged, ou chame StateHasChanged no componente
            }
        }


        /// <summary>
        /// Venda selecionada na interface.
        /// </summary>
        private VendaDto? _vendaSelecionada;
        public VendaDto? VendaSelecionada
        {
            get => _vendaSelecionada;
            set => SetProperty(ref _vendaSelecionada, value);
        }

        /// <summary>
        /// Dados para criação de nova venda.
        /// </summary>
        private CriarVendaDto _novaVenda = new();
        public CriarVendaDto NovaVenda
        {
            get => _novaVenda;
            set => SetProperty(ref _novaVenda, value);
        }

        /// <summary>
        /// Dados para edição de venda existente.
        /// </summary>
        private AtualizarVendaDto? _vendaEdicao;
        public AtualizarVendaDto? VendaEdicao
        {
            get => _vendaEdicao;
            set => SetProperty(ref _vendaEdicao, value);
        }

        /// <summary>
        /// Texto de pesquisa para filtrar vendas.
        /// </summary>
        private string _textoPesquisa = string.Empty;
        public string TextoPesquisa
        {
            get => _textoPesquisa;
            set
            {
                if (SetProperty(ref _textoPesquisa, value))
                {
                    _ = Task.Run(async () => await (PesquisarVendasAsync()));
                }
            }
        }


        /// <summary>
        /// Período para filtrar vendas.
        /// </summary>
        private DateRange? _periodoFiltro = new DateRange(DateTime.Today.AddDays(-30), DateTime.Today);
        public DateRange? PeriodoFiltro
        {
            get => _periodoFiltro;
            set
            {
                if (SetProperty(ref _periodoFiltro, value))
                {
                    _ = Task.Run(async () => await CarregarVendasAsync());
                }
            }
        }

        /// <summary>
        /// Estatísticas das vendas.
        /// </summary>
        private EstatisticasVendasDto? _estatisticasVendas; /// precisa implementar este DTO
        public EstatisticasVendasDto? EstatisticasVendas /// precisa implementar este DTO
        {
            get => _estatisticasVendas;
            set => SetProperty(ref _estatisticasVendas, value);
        }

        /// <summary>
        /// Indica se o modal de criação está aberto.
        /// </summary>
        private bool _modalCriacaoAberto;
        public bool ModalCriacaoAberto
        {
            get => _modalCriacaoAberto;
            set => SetProperty(ref _modalCriacaoAberto, value);
        }

        /// <summary>
        /// Indica se o modal de edição está aberto.
        /// </summary>
        private bool _modalEdicaoAberto;
        public bool ModalEdicaoAberto
        {
            get => _modalEdicaoAberto;
            set => SetProperty(ref _modalEdicaoAberto, value);
        }

        /// <summary>
        /// Construtor que recebe as dependências via injeção de dependência.
        /// </summary>
        /// <param name="apiService">Serviço para comunicação com a API.</param>
        public VendaViewModel(IVendaApiService apiService, IFuncionarioApiService funcionarioApiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _funcionarioApiService = funcionarioApiService ?? throw new ArgumentNullException(nameof(funcionarioApiService));
        }

        /// <summary>
        /// Carrega todas as vendas da API.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário para filtrar (opcional).</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task CarregarVendasAsync()
        {
            EstaCarregando = true;
            try
            {
                var resultado = await ExecutarOperacaoAsync(async () =>
                {
                    DateTime? dataInicio = PeriodoFiltro?.Start;
                    DateTime? dataFim = PeriodoFiltro?.End;
                    return await _apiService.ObterVendasAsync();
                });
                var vendas = await ExecutarOperacaoAsync(() => _apiService.ObterVendasAsync());

                if (vendas != null)
                {
                    _vendasOriginais = vendas.ToList(); // mantém lista completa
                    AtualizarListaFiltrada(_vendasOriginais);
                }
            }
            catch (Exception ex)
            {
                MensagemErro = "Erro ao carregar vendas.";
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        /// <summary>
        /// Carrega os funcionários disponíveis para seleção.
        /// </summary>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task CarregarFuncionariosDisponiveisAsync()
        {
            var resultado = await ExecutarOperacaoAsync(() =>
                _funcionarioApiService.ObterFuncionariosAsync()
            );

            if (resultado is not null && resultado.Sucesso && resultado.Dados is not null)
            {
                FuncionariosDisponiveis.Clear();
                foreach (var funcionario in resultado.Dados)
                {
                    FuncionariosDisponiveis.Add(funcionario);
                }
            }
        }


        /// <summary>
        /// Carrega as estatísticas das vendas.
        /// </summary>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task CarregarEstatisticasAsync()
        {
            var estatisticas = await ExecutarOperacaoAsync(async () =>
            {
                DateTime? dataInicio = PeriodoFiltro?.Start;
                DateTime? dataFim = PeriodoFiltro?.End;
                return await _apiService.ObterEstatisticasVendasAsync(dataInicio, dataFim);
            });

            if (estatisticas != null)
            {
                EstatisticasVendas = estatisticas;
            }
        }

        /// <summary>
        /// Pesquisa vendas por texto.
        /// </summary>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task PesquisarVendasAsync()
        {
            if (string.IsNullOrWhiteSpace(TextoPesquisa))
            {
                AtualizarListaFiltrada(_vendasOriginais);
                return;
            }

            var filtro = TextoPesquisa.Trim();

            var filtradas = _vendasOriginais
                .Where(v =>
                    (v.NomeFuncionario?.Contains(filtro, StringComparison.OrdinalIgnoreCase) ?? false)
                    || v.Descricao?.Contains(filtro, StringComparison.OrdinalIgnoreCase) == true
                    || v.Valor.ToString("C").Contains(filtro, StringComparison.OrdinalIgnoreCase))
                .ToList();

            AtualizarListaFiltrada(filtradas);
            await Task.CompletedTask;
        }

        private void AtualizarListaFiltrada(List<VendaDto> vendas)
        {
            VendasFiltradas = new List<VendaDto>(vendas);
        }

        /// <summary>
        /// Abre o modal para criar uma nova venda.
        /// </summary>
        /// <param name="funcionarioId">ID do funcionário pré-selecionado (opcional).</param>
        public void AbrirModalCriacao(int? funcionarioId = null)
        {
            NovaVenda = new CriarVendaDto
            {
                FuncionarioId = funcionarioId ?? (FuncionariosDisponiveis.FirstOrDefault()?.Id ?? 1),
                DataVenda = DateTime.Today,
                Valor = 1,
                Descricao = string.Empty
            };
            ModalCriacaoAberto = true;
            MensagemErro = null;
        }

        /// <summary>
        /// Fecha o modal de criação.
        /// </summary>
        public void FecharModalCriacao()
        {
            ModalCriacaoAberto = false;
            NovaVenda = new CriarVendaDto();
            LimparMensagens();
        }

        /// <summary>
        /// Abre o modal para editar uma venda existente.
        /// </summary>
        /// <param name="venda">Venda a ser editada.</param>
        public void AbrirModalEdicao(VendaDto venda)
        {
            if (venda == null) return;

            VendaEdicao = new AtualizarVendaDto
            {
                Id = venda.Id,
                FuncionarioId = venda.FuncionarioId,
                Valor = venda.Valor,
                DataVenda = venda.DataVenda,
                Descricao = venda.Descricao
            };

            ModalEdicaoAberto = true;
            MensagemErro = null;
        }

        /// <summary>
        /// Fecha o modal de edição.
        /// </summary>
        public void FecharModalEdicao()
        {
            ModalEdicaoAberto = false;
            VendaEdicao = null;
            LimparMensagens();
        }

        /// <summary>
        /// Cria uma nova venda.
        /// </summary>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task<ResultadoOperacao> CriarVendaAsync()
        {
            if (NovaVenda == null)
                return ResultadoOperacao.CriarFalha("Preencha os dados da nova venda.");

            var resultadoValidacao = await ValidarDtoResultadoAsync(_validadorCriarVenda, NovaVenda);
            if (!resultadoValidacao.Sucesso)
                return resultadoValidacao;

            try
            {
                EstaCarregando = true;
                await _apiService.CriarVendaAsync(NovaVenda);
                FecharModalCriacao();
                await CarregarVendasAsync();

                return ResultadoOperacao.CriarSucesso("Venda criada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResultadoOperacao.CriarFalha($"Erro ao criar venda: {ex.Message}");
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        /// <summary>
        /// Atualiza uma venda existente.
        /// </summary>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task<ResultadoOperacao> AtualizarVendaAsync()
        {
            if (VendaEdicao is null)
                return ResultadoOperacao.CriarFalha("Venda inválida.");

            var dto = new AtualizarVendaDto
            {
                Id = VendaEdicao.Id,
                FuncionarioId = VendaEdicao.FuncionarioId,
                Valor = VendaEdicao.Valor,
                DataVenda = VendaEdicao.DataVenda,
                Descricao = VendaEdicao.Descricao
            };

            var resultadoValidacao = await ValidarDtoResultadoAsync(_validadorAtualizarVenda, dto);
            if (!resultadoValidacao.Sucesso)
                return resultadoValidacao;

            try
            {
                EstaCarregando = true;

                await _apiService.AtualizarVendaAsync(dto);
                FecharModalEdicao();
                await CarregarVendasAsync();

                return ResultadoOperacao.CriarSucesso("Venda atualizada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResultadoOperacao.CriarFalha($"Erro ao atualizar meta: {ex.Message}");
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        /// <summary>
        /// Remove uma venda.
        /// </summary>
        /// <param name="venda">Venda a ser removida.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task<bool> RemoverVendaAsync(VendaDto venda)
        {
            if (venda == null)
            {
                DefinirErro("Nenhuma venda selecionada para remoção.");
                return false;
            }

            var sucesso = await ExecutarOperacaoAsync(async () => 
            {
                await _apiService.RemoverVendaAsync(venda.Id);
                return true;
            }, "Venda removida com sucesso!");

            if (sucesso)
            {
                await CarregarVendasAsync();
            }

            return sucesso;
        }

        /// <summary>
        /// Seleciona uma venda.
        /// </summary>
        /// <param name="venda">Venda a ser selecionada.</param>
        public void SelecionarVenda(VendaDto venda)
        {
            VendaSelecionada = venda;
        }

        /// <summary>
        /// Limpa a seleção de venda.
        /// </summary>
        public void LimparSelecao()
        {
            VendaSelecionada = null;
        }
    }
}

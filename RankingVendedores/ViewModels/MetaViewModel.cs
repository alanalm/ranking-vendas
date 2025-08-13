using Aplicacao.Utils;
using MudBlazor;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Validacoes;
using RankingVendedores.Servicos.Interfaces;
using System.Collections.ObjectModel;

namespace RankingVendedores.ViewModels
{
    public class MetaViewModel : ViewModelBase
    {
        private readonly IMetaApiService _apiService;
        private readonly IIndicadorApiService _indicadorApiService;
        private readonly ValidadorCriarMeta _validadorCriarMeta = new();

        public List<MetaDto> Metas { get; private set; } = new();

        // Lista original carregada da API
        private List<MetaDto> _metasOriginais = new();

        // Lista filtrada para exibição (binding)
        public ObservableCollection<MetaDto> MetasFiltradas { get; private set; } = new();

        public ObservableCollection<IndicadorDto> IndicadoresDisponiveis { get; } = new();

        private MetaDto? _metaSelecionada;
        public MetaDto? MetaSelecionada
        {
            get => _metaSelecionada;
            set => SetProperty(ref _metaSelecionada, value);
        }

        private CriarMetaDto _novaMeta = new();
        public CriarMetaDto NovaMeta
        {
            get => _novaMeta;
            set => SetProperty(ref _novaMeta, value);
        }

        private AtualizarMetaDto? _metaEdicao;
        public AtualizarMetaDto? MetaEdicao
        {
            get => _metaEdicao;
            set => SetProperty(ref _metaEdicao, value);
        }

        private string _textoPesquisa = string.Empty;
        public string TextoPesquisa
        {
            get => _textoPesquisa;
            set
            {
                if (SetProperty(ref _textoPesquisa, value))
                {
                    
                }
            }
        }

        private bool _modalCriacaoAberto;
        public bool ModalCriacaoAberto
        {
            get => _modalCriacaoAberto;
            set => SetProperty(ref _modalCriacaoAberto, value);
        }

        private bool _modalEdicaoAberto;
        public bool ModalEdicaoAberto
        {
            get => _modalEdicaoAberto;
            set => SetProperty(ref _modalEdicaoAberto, value);
        }

        private bool _estaCarregando;
        public bool EstaCarregando
        {
            get => _estaCarregando;
            set => SetProperty(ref _estaCarregando, value);
        }

        public string? MensagemErro { get; private set; }

        public MetaViewModel(IMetaApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task CarregarMetasAsync(int? indicadorId = null)
        {
            MensagemErro = null;

            try
            {
                EstaCarregando = true;

                var resultado = await _apiService.ObterMetasComResultadoAsync();

                if (!resultado.Sucesso)
                {
                    MensagemErro = resultado.Mensagem ?? "Erro ao carregar metas.";
                    return;
                }

                var metas = resultado.Dados ?? new List<MetaDto>();

                _metasOriginais = metas;

                if (indicadorId.HasValue && indicadorId.Value > 0)
                {
                    var metasFiltradas = metas
                        .Where(m => m.IndicadorId == indicadorId.Value)
                        .ToList();

                    AtualizarListaFiltrada(metasFiltradas);
                }
                else
                {
                    AtualizarListaFiltrada(metas);
                }
            }
            catch (Exception ex)
            {
                MensagemErro = $"Erro inesperado ao carregar metas: {ex.Message}";
            }
            finally
            {
                EstaCarregando = false;
            }
        }


        public async Task CarregarIndicadoresDisponiveisAsync()
        {
            try
            {
                var indicadores = await _indicadorApiService.ObterIndicadoresAsync();
                IndicadoresDisponiveis.Clear();

                foreach (var indicador in indicadores.Dados)
                {
                    IndicadoresDisponiveis.Add(indicador);
                }
            }
            catch (Exception ex)
            {
                MensagemErro = $"Erro ao carregar indicadores: {ex.Message}";
            }
        }

        public async Task PesquisarMetasAsync()
        {
            if (string.IsNullOrWhiteSpace(TextoPesquisa))
            {
                AtualizarListaFiltrada(_metasOriginais);
                return;
            }

            var filtro = TextoPesquisa.Trim();

            var filtradas = _metasOriginais
                .Where(m =>
                    (m.NomeIndicador?.Contains(filtro, StringComparison.OrdinalIgnoreCase) ?? false)
                    || m.Valor.ToString("C").Contains(filtro, StringComparison.OrdinalIgnoreCase))
                .ToList();

            AtualizarListaFiltrada(filtradas);
            await Task.CompletedTask;
        }

        private void AtualizarListaFiltrada(List<MetaDto> metas)
        {
            MetasFiltradas.Clear();
            foreach (var meta in metas)
            {
                MetasFiltradas.Add(meta);
            }
        }

        public void AbrirModalCriacao(int? indicadorId = null)
        {
            MetaEdicao = new AtualizarMetaDto
            {
                IndicadorId = indicadorId ?? 0,
                DataInicio = DateTime.Today,
                Ativa = true
            };
            ModalCriacaoAberto = true;
            MensagemErro = null;
        }

        public void FecharModalCriacao()
        {
            ModalCriacaoAberto = false;
            NovaMeta = new CriarMetaDto();
            MensagemErro = null;
        }

        public void AbrirModalEdicao(MetaDto meta)
        {
            if (meta == null) return;

            MetaEdicao = new AtualizarMetaDto
            {
                Id = meta.Id,
                IndicadorId = meta.IndicadorId,
                Valor = meta.Valor,
                DataInicio = meta.DataInicio,
                DataFim = meta.DataFim,
                Ativa = meta.Ativa
            };

            ModalEdicaoAberto = true;
            MensagemErro = null;
        }

        public void FecharModalEdicao()
        {
            ModalEdicaoAberto = false;
            MetaEdicao = null;
            MensagemErro = null;
        }

        public async Task<ResultadoOperacao> CriarMetaAsync()
        {
            if (NovaMeta == null)
                return ResultadoOperacao.CriarFalha("Preencha os dados da nova meta.");

            var resultadoValidacao = _validadorCriarMeta.Validate(NovaMeta);
            if (!resultadoValidacao.IsValid)
            {
                var mensagem = resultadoValidacao.Errors.First().ErrorMessage;
                return ResultadoOperacao.CriarFalha(mensagem);
            }

            try
            {
                EstaCarregando = true;

                await _apiService.CriarMetaAsync(NovaMeta);
                FecharModalCriacao();
                await CarregarMetasAsync();

                return ResultadoOperacao.CriarSucesso("Meta criada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResultadoOperacao.CriarFalha($"Erro ao criar meta: {ex.Message}");
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        public async Task<ResultadoOperacao> AtualizarMetaAsync()
        {
            if (MetaEdicao is null)
                return ResultadoOperacao.CriarFalha("Meta inválida.");

            var dto = new AtualizarMetaDto
            {
                Id = MetaEdicao.Id,
                IndicadorId = MetaEdicao.IndicadorId,
                Valor = MetaEdicao.Valor,
                DataInicio = MetaEdicao.DataInicio,
                DataFim = MetaEdicao.DataFim,
                Ativa = MetaEdicao.Ativa
            };

            try
            {
                EstaCarregando = true;

                await _apiService.AtualizarMetaAsync(dto);
                FecharModalEdicao();
                await CarregarMetasAsync();

                return ResultadoOperacao.CriarSucesso("Meta atualizada com sucesso!");
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

        public async Task<bool> RemoverMetaAsync(MetaDto meta)
        {
            MensagemErro = null;

            if (meta == null)
            {
                MensagemErro = "Meta inválida.";
                return false;
            }

            try
            {
                EstaCarregando = true;
                await _apiService.RemoverMetaAsync(meta.Id);
                await CarregarMetasAsync();
                return true;
            }
            catch (Exception ex)
            {
                MensagemErro = $"Erro ao remover meta: {ex.Message}";
                return false;
            }
            finally
            {
                EstaCarregando = false;
            }
        }
    }
}

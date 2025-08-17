using Aplicacao.Utils;
using MudBlazor;
using Ranking.Aplicacao.DTOs;
using Ranking.Aplicacao.Validacoes;
using RankingVendedores.Pages;
using RankingVendedores.Servicos.Interfaces;
using System.Collections.ObjectModel;

namespace RankingVendedores.ViewModels
{
    public class FuncionarioViewModel : ViewModelBase
    {
        private readonly IFuncionarioApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly ValidadorCriarFuncionario _validadorCriarFuncionario = new();
        private readonly ValidadorAtualizarFuncionario _validadorAtualizarFuncionario = new();

        public string? FiltroNome { get; set; }
        public ObservableCollection<FuncionarioDto> FuncionariosFiltrados { get; set; } = new();
        public ObservableCollection<FuncionarioDto> Funcionarios { get; set; } = new();

        private FuncionarioDto? _funcionarioSelecionado;
        public FuncionarioDto? FuncionarioSelecionado
        {
            get => _funcionarioSelecionado;
            set => SetProperty(ref _funcionarioSelecionado, value);
        }

        public FuncionarioViewModel(IFuncionarioApiService apiService, IDialogService dialogService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public async Task CarregarFuncionariosAsync()
        {
            var resultado = await ExecutarOperacaoAsync(async () =>
                await _apiService.ObterFuncionariosAsync());

            if (resultado?.Sucesso == true && resultado.Dados != null)
            {
                Funcionarios.Clear();
                foreach (var f in resultado.Dados)
                    Funcionarios.Add(f);
            }
            if (resultado.Sucesso && resultado.Dados is not null)
            {
                Funcionarios = new ObservableCollection<FuncionarioDto>(resultado.Dados);
                FuncionariosFiltrados = new ObservableCollection<FuncionarioDto>(resultado.Dados);
            }
        }

        public void FiltrarFuncionarios()
        {
            if (string.IsNullOrWhiteSpace(FiltroNome))
            {
                FuncionariosFiltrados = new ObservableCollection<FuncionarioDto>(Funcionarios);
            }
            else
            {
                var filtro = FiltroNome.Trim().ToLower();
                FuncionariosFiltrados = new ObservableCollection<FuncionarioDto>(
                    Funcionarios.Where(f => f.Nome.ToLower().Contains(filtro))
                );
            }
        }

        public async Task PesquisarFuncionariosAsync()
        {
            var resultado = await ExecutarOperacaoAsync(async () =>
                await _apiService.PesquisarFuncionariosAsync(TextoPesquisa));

            if (resultado != null)
            {
                Funcionarios.Clear();
                foreach (var f in resultado)
                    Funcionarios.Add(f);
            }
        }

        private string _textoPesquisa = string.Empty;
        public string TextoPesquisa
        {
            get => _textoPesquisa;
            set
            {
                if (SetProperty(ref _textoPesquisa, value))
                    _ = Task.Run(PesquisarFuncionariosAsync);
            }
        }

        public async Task AbrirModalNovoFuncionarioAsync()
        {
            var parameters = new DialogParameters();

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                BackdropClick = true
            };

            var dialog = _dialogService.Show<FuncionarioModal>("Novo Funcionário", parameters, options);
            var resultado = await dialog.Result;

            if (!resultado.Canceled && resultado.Data is FuncionarioDto novoFuncionario)
            {
                await CarregarFuncionariosAsync();
            }
        }

        public async Task<ResultadoOperacao<FuncionarioDto>> CriarFuncionarioAsync(CriarFuncionarioDto dto)
        {
            var resultadoValidacao = await _validadorCriarFuncionario.ValidateAsync(dto);

            if (!ValidarDto(_validadorCriarFuncionario, dto))
                return ResultadoOperacao<FuncionarioDto>.CriarFalha("Dados inválidos");

            var resultado = await ExecutarOperacaoAsync(async () =>
            {
                var funcionarioCriado = await _apiService.CriarFuncionarioAsync(dto);
                return funcionarioCriado;
            }, "Funcionário criado com sucesso!");

            if (resultado.Sucesso)
                await CarregarFuncionariosAsync();

            return resultado;
        }

        public async Task<ResultadoOperacao<FuncionarioDto>> AtualizarFuncionarioAsync(AtualizarFuncionarioDto dto)
        {
            var resultadoValidacao = await _validadorAtualizarFuncionario.ValidateAsync(dto);

            if (!ValidarDto(_validadorAtualizarFuncionario, dto))
                return ResultadoOperacao<FuncionarioDto>.CriarFalha("Dados inválidos");

            var resultado = await ExecutarOperacaoAsync(async () =>
            {
                var funcionarioAtualizado = await _apiService.AtualizarFuncionarioAsync(dto);
                return funcionarioAtualizado;
            }, "Funcionário atualizado com sucesso!");

            if (resultado.Sucesso)
                await CarregarFuncionariosAsync();

            return resultado;
        }


        public async Task<bool> RemoverFuncionarioAsync(FuncionarioDto funcionario)
        {
            if (funcionario == null)
            {
                DefinirErro("Nenhum funcionário selecionado para remoção.");
                return false;
            }

            var podeRemover = await ExecutarOperacaoAsync(() =>
                _apiService.PodeFuncionarioSerRemovidoAsync(funcionario.Id));

            if (podeRemover != true)
            {
                DefinirErro("Funcionário não pode ser removido pois possui vendas associadas.");
                return false;
            }

            var sucesso = await ExecutarOperacaoAsync(async () =>
            {
                await _apiService.RemoverFuncionarioAsync(funcionario.Id);
                return true;
            }, "Funcionário removido com sucesso!");

            if (sucesso)
                await CarregarFuncionariosAsync();

            return sucesso;
        }

        public void SelecionarFuncionario(FuncionarioDto funcionario) =>
            FuncionarioSelecionado = funcionario;

        public void LimparSelecao() =>
            FuncionarioSelecionado = null;
    }
}
using Aplicacao.Utils;
using FluentValidation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RankingVendedores.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public IEnumerable<string> MensagensErro => _erros.SelectMany(e => e.Value);
        /// Evento disparado quando uma propriedade é alterada.

        public event PropertyChangedEventHandler? PropertyChanged;

        // Dicionário para armazenar erros de validação por propriedade
        private readonly Dictionary<string, List<string>> _erros = new();

        // Evento para notificar alterações de erros (opcional, útil para UI)
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        // Indica se há erros de validação
        public bool TemErros => _erros.Count > 0;

        /// Indica se o ViewModel está carregando dados.
        private bool _estaCarregando;
        public bool EstaCarregando
        {
            get => _estaCarregando;
            set => SetProperty(ref _estaCarregando, value);
        }

        /// Mensagem de erro atual.
        private string? _mensagemErro;
        public string? MensagemErro
        {
            get => _mensagemErro;
            set => SetProperty(ref _mensagemErro, value);
        }

        /// Indica se há erro no ViewModel.
        public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

        /// Mensagem de sucesso atual.
        private string? _mensagemSucesso;
        public string? MensagemSucesso
        {
            get => _mensagemSucesso;
            set => SetProperty(ref _mensagemSucesso, value);
        }

        /// Indica se há mensagem de sucesso no ViewModel.
        public bool TemSucesso => !string.IsNullOrEmpty(MensagemSucesso);

        /// Define o valor de uma propriedade e dispara o evento PropertyChanged se necessário.
        /// <typeparam name="T">Tipo da propriedade.</typeparam>
        /// <param name="field">Campo de apoio da propriedade.</param>
        /// <param name="value">Novo valor da propriedade.</param>
        /// <param name="propertyName">Nome da propriedade (preenchido automaticamente).</param>
        /// <returns>True se o valor foi alterado, false caso contrário.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// Dispara o evento PropertyChanged para a propriedade especificada.
        /// <param name="propertyName">Nome da propriedade (preenchido automaticamente).</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// Limpa as mensagens de erro e sucesso.
        protected void LimparMensagens()
        {
            MensagemErro = null;
            MensagemSucesso = null;
        }

        /// Define uma mensagem de erro e limpa a mensagem de sucesso.
        /// <param name="mensagem">Mensagem de erro.</param>
        protected void DefinirErro(string mensagem)
        {
            MensagemErro = mensagem;
            MensagemSucesso = null;
        }

        /// Define uma mensagem de sucesso e limpa a mensagem de erro.
        /// <param name="mensagem">Mensagem de sucesso.</param>
        protected void DefinirSucesso(string mensagem)
        {
            MensagemSucesso = mensagem;
            MensagemErro = null;
        }

        /// Executa uma operação assíncrona com tratamento de erro e indicador de carregamento.
        /// <param name="operacao">Operação a ser executada.</param>
        /// <param name="mensagemSucesso">Mensagem de sucesso (opcional).</param>
        /// <returns>True se a operação foi bem-sucedida, false caso contrário.</returns>
        protected async Task<bool> ExecutarOperacaoAsync(Func<Task<ResultadoOperacao>> operacao)
        {
            try
    {
        EstaCarregando = true;
        LimparMensagens();

        var resultado = await operacao();

        if (resultado.Sucesso)
        {
            if (!string.IsNullOrWhiteSpace(resultado.Mensagem))
                DefinirSucesso(resultado.Mensagem);
            return true;
        }
        else
        {
            DefinirErro(resultado.Mensagem ?? "Ocorreu um erro.");
            return false;
        }
    }
    catch (Exception ex)
    {
        DefinirErro($"Erro: {ex.Message}");
        return false;
    }
    finally
    {
        EstaCarregando = false;
    }
        }

        /// Executa uma operação assíncrona com retorno e tratamento de erro.
        /// <typeparam name="T">Tipo do retorno da operação.</typeparam>
        /// <param name="operacao">Operação a ser executada.</param>
        /// <param name="mensagemSucesso">Mensagem de sucesso (opcional).</param>
        /// <returns>Resultado da operação ou valor padrão em caso de erro.</returns>
        protected async Task<T?> ExecutarOperacaoAsync<T>(Func<Task<T>> operacao, string? mensagemSucesso = null)
        {
            try
            {
                EstaCarregando = true;
                LimparMensagens();

                var resultado = await operacao();

                if (!string.IsNullOrEmpty(mensagemSucesso))
                    DefinirSucesso(mensagemSucesso);

                return resultado;
            }
            catch (Exception ex)
            {
                DefinirErro($"Erro: {ex.Message}");
                return default;
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        protected async Task<ResultadoOperacao<T>?> ExecutarOperacaoComResultadoAsync<T>(Func<Task<ResultadoOperacao<T>>> operacao)
        {
            try
            {
                EstaCarregando = true;
                LimparMensagens();

                var resultado = await operacao();

                if (resultado.Sucesso)
                {
                    if (!string.IsNullOrWhiteSpace(resultado.Mensagem))
                        DefinirSucesso(resultado.Mensagem);
                }
                else
                {
                    DefinirErro(resultado.Mensagem ?? "Ocorreu um erro.");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                DefinirErro($"Erro: {ex.Message}");
                return null;
            }
            finally
            {
                EstaCarregando = false;
            }
        }

        // Retorna erros de uma propriedade, ou null se não houver
        public IEnumerable<string>? GetErros(string propriedade)
        {
            if (string.IsNullOrEmpty(propriedade))
                return null;

            return _erros.ContainsKey(propriedade) ? _erros[propriedade] : null;
        }

        // Método que deve ser chamado por subclasses para definir erros
        protected void DefinirErroValidacao(string propriedade, string erro)
        {
            if (!_erros.ContainsKey(propriedade))
                _erros[propriedade] = new List<string>();

            if (!_erros[propriedade].Contains(erro))
            {
                _erros[propriedade].Add(erro);
                OnErrorsChanged(propriedade);
                OnPropertyChanged(nameof(TemErros));
            }
        }

        // Limpa os erros de uma propriedade
        protected void LimparErros(string propriedade)
        {
            if (_erros.Remove(propriedade))
            {
                OnErrorsChanged(propriedade);
                OnPropertyChanged(nameof(TemErros));
            }
        }

        // Limpa todos os erros
        protected void LimparTodosErros()
        {
            var propriedades = _erros.Keys.ToList();
            _erros.Clear();
            foreach (var prop in propriedades)
            {
                OnErrorsChanged(prop);
            }
            OnPropertyChanged(nameof(TemErros));
        }

        protected virtual void OnErrorsChanged(string propriedade)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propriedade));
        }

        public bool ValidarDto<T>(AbstractValidator<T> validador, T dto)
        {
            LimparTodosErros();

            var resultado = validador.Validate(dto);

            if (!resultado.IsValid)
            {
                foreach (var erro in resultado.Errors)
                {
                    DefinirErroValidacao(erro.PropertyName, erro.ErrorMessage);
                }

                return false;
            }

            return true;
        }
        public IEnumerable<string> GetPropriedadesComErros()
        {
            return _erros.Keys.ToList();
        }
        public ResultadoOperacao ValidarDtoResultado<T>(AbstractValidator<T> validador, T dto)
        {
            LimparTodosErros();

            var resultadoValidacao = validador.Validate(dto);

            if (!resultadoValidacao.IsValid)
            {
                foreach (var erro in resultadoValidacao.Errors)
                {
                    DefinirErroValidacao(erro.PropertyName, erro.ErrorMessage);
                }

                var mensagens = resultadoValidacao.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return ResultadoOperacao.CriarFalha(mensagens);
            }

            return ResultadoOperacao.CriarSucesso("Validação concluída.");
        }

        public async Task<ResultadoOperacao> ValidarDtoResultadoAsync<T>(IValidator<T> validador, T dto)
        {
            LimparTodosErros();

            var resultadoValidacao = await validador.ValidateAsync(dto);

            if (!resultadoValidacao.IsValid)
            {
                foreach (var erro in resultadoValidacao.Errors)
                {
                    DefinirErroValidacao(erro.PropertyName, erro.ErrorMessage);
                }

                var mensagens = resultadoValidacao.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return ResultadoOperacao.CriarFalha(mensagens);
            }

            return ResultadoOperacao.CriarSucesso("Validação concluída.");
        }
    }
}

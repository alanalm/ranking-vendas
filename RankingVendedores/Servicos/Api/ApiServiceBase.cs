using Aplicacao.Utils;
using System.Net.Http.Json;
using System.Text.Json;

namespace RankingVendedores.Servicos.Api
{
    public abstract class ApiServiceBase
    {
        /// Implementação do serviço de comunicação com a API.
        /// Implementa todas as operações de comunicação HTTP com a API backend.
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;

        /// Construtor que recebe o HttpClient via injeção de dependência.
        /// <param name="httpClient">Cliente HTTP configurado.</param>
        protected ApiServiceBase(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        protected async Task<ResultadoOperacao<T>> TratarRespostaApi<T>(HttpResponseMessage response, string mensagemErro)
        {
            if (!response.IsSuccessStatusCode)
            {
                return new ResultadoOperacao<T>
                {
                    Sucesso = false,
                    Mensagem = mensagemErro,
                    Dados = default
                };
            }

            var resultado = await response.Content.ReadFromJsonAsync<ResultadoOperacao<T>>(_jsonOptions);

            return resultado ?? new ResultadoOperacao<T>
            {
                Sucesso = false,
                Mensagem = "Erro ao processar resposta da API.",
                Dados = default
            };
        }

        // Nova: específica para respostas do tipo ResultadoOperacao<T>
        protected async Task<ResultadoOperacao<T>> TratarRespostaResultado<T>(HttpResponseMessage response, string mensagemErroPadrao)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var resultado = JsonSerializer.Deserialize<ResultadoOperacao<T>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return resultado!;
            }
            catch
            {
                throw new Exception(mensagemErroPadrao);
            }
        }
    }
}


using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;
using RankingVendedores.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RankingVendedores.Services
{
    /// Implementação do serviço de comunicação com a API.
    /// Implementa todas as operações de comunicação HTTP com a API backend.
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// Construtor que recebe o HttpClient via injeção de dependência.
        /// <param name="httpClient">Cliente HTTP configurado.</param>
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<DTOs.ResultadoOperacao<T>> TratarRespostaApi<T>(HttpResponseMessage response, string mensagemErro)
        {
            if (!response.IsSuccessStatusCode)
            {
                return new DTOs.ResultadoOperacao<T>
                {
                    Sucesso = false,
                    Mensagem = mensagemErro,
                    Dados = default
                };
            }

            var resultado = await response.Content.ReadFromJsonAsync<DTOs.ResultadoOperacao<T>>(_jsonOptions);

            return resultado ?? new DTOs.ResultadoOperacao<T>
            {
                Sucesso = false,
                Mensagem = "Erro ao processar resposta da API.",
                Dados = default
            };
        }

        // Nova: específica para respostas do tipo ResultadoOperacao<T>
        public async Task<DTOs.ResultadoOperacao<T>> TratarRespostaResultado<T>(HttpResponseMessage response, string mensagemErroPadrao)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var resultado = JsonSerializer.Deserialize<DTOs.ResultadoOperacao<T>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return resultado!;
            }
            catch (Exception ex)
            {
                throw new Exception(mensagemErroPadrao);
            }
        }

        #region Funcionários

        public async Task<DTOs.ResultadoOperacao<List<FuncionarioDto>>> ObterFuncionariosAsync()
        {
            var response = await _httpClient.GetAsync("api/funcionarios");

            if (!response.IsSuccessStatusCode)
                return new DTOs.ResultadoOperacao<List<FuncionarioDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao consultar a API.",
                    ErrosValidacao = new List<string> { "Erro ao consultar a API." }
                };

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var resultado = await response.Content.ReadFromJsonAsync<DTOs.ResultadoOperacao<List<FuncionarioDto>>>(options);

            return resultado ?? new DTOs.ResultadoOperacao<List<FuncionarioDto>>
            {
                Sucesso = false,
                Mensagem = "Resposta nula.",
                ErrosValidacao = new List<string> { "Resposta nula." }
            };
        }

        public async Task<FuncionarioDto?> ObterFuncionarioPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/funcionarios/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FuncionarioDto>(_jsonOptions);
            }
            return null;
        }

        public async Task<List<FuncionarioDto>> PesquisarFuncionariosAsync(string? termo)
        {
            var response = await _httpClient.GetAsync($"api/funcionarios?pesquisa={termo}");

            if (!response.IsSuccessStatusCode)
                return new List<FuncionarioDto>();

            var resultado = await response.Content.ReadFromJsonAsync<DTOs.ResultadoOperacao<List<FuncionarioDto>>>();

            return resultado?.Dados ?? new List<FuncionarioDto>();
        }


        public async Task<FuncionarioDto> CriarFuncionarioAsync(CriarFuncionarioDto funcionario)
        {
            var response = await _httpClient.PostAsJsonAsync("api/funcionarios", funcionario, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var funcionarioCriado = await response.Content.ReadFromJsonAsync<FuncionarioDto>(_jsonOptions);
            return funcionarioCriado ?? throw new InvalidOperationException("Erro ao criar funcionário");
        }

        public async Task<FuncionarioDto> AtualizarFuncionarioAsync(AtualizarFuncionarioDto funcionario)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/funcionarios/{funcionario.Id}", funcionario, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var funcionarioAtualizado = await response.Content.ReadFromJsonAsync<FuncionarioDto>(_jsonOptions);
            return funcionarioAtualizado ?? throw new InvalidOperationException("Erro ao atualizar funcionário");
        }

        public async Task RemoverFuncionarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/funcionarios/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> PodeFuncionarioSerRemovidoAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/funcionarios/{id}/pode-remover");
                response.EnsureSuccessStatusCode();

                var resultado = await response.Content.ReadFromJsonAsync<DTOs.ResultadoOperacao<bool>>();
                return resultado?.Dados ?? false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar remoção: {ex.Message}");
                return false; 
            }
        }

        #endregion

        #region Indicadores

        public async Task<DTOs.ResultadoOperacao<List<IndicadorDto>>> ObterIndicadoresAsync()
        {
            var response = await _httpClient.GetAsync("api/indicadores");
            return await TratarRespostaApi<List<IndicadorDto>>(response, "Erro ao obter indicadores.");
        }

        public async Task<DTOs.ResultadoOperacao<IndicadorDto>> ObterIndicadorPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/indicadores/{id}");
            return await TratarRespostaApi<IndicadorDto>(response, $"Erro ao obter indicador com ID {id}.");
        }

        public async Task<DTOs.ResultadoOperacao<IEnumerable<IndicadorDto>>> PesquisarIndicadoresAsync(string nome)
        {
            var response = await _httpClient.GetAsync($"api/indicadores/pesquisar?nome={Uri.EscapeDataString(nome)}");
            return await TratarRespostaApi<IEnumerable<IndicadorDto>>(response, "Erro ao pesquisar indicadores.");
        }

        public async Task<DTOs.ResultadoOperacao<IndicadorDto>> CriarIndicadorAsync(CriarIndicadorDto indicador)
        {
            var response = await _httpClient.PostAsJsonAsync("api/indicadores", indicador, _jsonOptions);
            return await TratarRespostaApi<IndicadorDto>(response, "Erro ao criar indicador.");
        }

        public async Task<DTOs.ResultadoOperacao<IndicadorDto>> AtualizarIndicadorAsync(AtualizarIndicadorDto indicador)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/indicadores/{indicador.Id}", indicador, _jsonOptions);
            return await TratarRespostaApi<IndicadorDto>(response, "Erro ao atualizar indicador.");
        }

        public async Task<DTOs.ResultadoOperacao<bool>> RemoverIndicadorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/indicadores/{id}");
            return await TratarRespostaApi<bool>(response, "Erro ao remover indicador.");
        }

        public async Task<DTOs.ResultadoOperacao<bool>> PodeIndicadorSerRemovidoAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/indicadores/{id}/pode-remover");
            return await TratarRespostaApi<bool>(response, "Erro ao verificar se o indicador pode ser removido.");
        }


        #endregion

        #region Metas

        public async Task<List<MetaDto>> ObterMetasAsync()
        {
            var resultado = await ObterMetasComResultadoAsync();

            if (resultado.Sucesso && resultado.Dados != null)
                return resultado.Dados;

            return new List<MetaDto>();
        }

        public async Task<DTOs.ResultadoOperacao<List<MetaDto>>> ObterMetasComResultadoAsync()
        {

            var response = await _httpClient.GetAsync("api/metas");

            var resultado = await TratarRespostaResultado<List<MetaDto>>(response, "Erro ao obter metas da API.");

            return resultado;
        }


        public async Task<DTOs.ResultadoOperacao<MetaDto>> ObterMetaPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/metas/{id}");
            return await TratarRespostaApi<MetaDto>(response, $"Erro ao obter meta com ID {id}.");
        }

        public async Task<DTOs.ResultadoOperacao<List<MetaDto>>> ObterMetasPorIndicadorAsync(int indicadorId)
        {
            var response = await _httpClient.GetAsync($"api/metas/indicador/{indicadorId}");
            return await TratarRespostaApi<List<MetaDto>>(response, $"Erro ao obter metas do indicador {indicadorId}.");
        }

        public async Task<DTOs.ResultadoOperacao<MetaDto>> CriarMetaAsync(CriarMetaDto meta)
        {
            var response = await _httpClient.PostAsJsonAsync("api/metas", meta, _jsonOptions);
            return await TratarRespostaApi<MetaDto>(response, "Erro ao criar meta.");
        }

        public async Task<DTOs.ResultadoOperacao<MetaDto>> AtualizarMetaAsync(AtualizarMetaDto meta)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/metas/{meta.Id}", meta, _jsonOptions);
            return await TratarRespostaApi<MetaDto>(response, "Erro ao atualizar meta.");
        }

        public async Task RemoverMetaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/metas/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<DTOs.ResultadoOperacao<bool>> AtivarMetaAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/metas/{id}/ativar", null);
            return await TratarRespostaApi<bool>(response, "Erro ao ativar meta.");
        }

        public async Task<DTOs.ResultadoOperacao<bool>> DesativarMetaAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/metas/{id}/desativar", null);
            return await TratarRespostaApi<bool>(response, "Erro ao desativar meta.");
        }

        #endregion

        #region Vendas

        public async Task<IEnumerable<VendaDto>> ObterVendasAsync()
        {
            var response = await _httpClient.GetAsync("api/vendas");
            var resultado = await TratarRespostaResultado<IEnumerable<VendaDto>>(response, "Erro ao obter vendas.");

            return resultado.Sucesso && resultado.Dados != null
                ? resultado.Dados
                : Enumerable.Empty<VendaDto>();
        }

        public async Task<VendaDto?> ObterVendaPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/vendas/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VendaDto>(_jsonOptions);
            }
            return null;
        }

        public async Task<IEnumerable<VendaDto>> ObterVendasPorFuncionarioAsync(int funcionarioId)
        {
            var response = await _httpClient.GetAsync($"api/vendas/funcionario/{funcionarioId}");
            response.EnsureSuccessStatusCode();

            var resultado = await response.Content.ReadFromJsonAsync<DTOs.ResultadoOperacao<IEnumerable<VendaDto>>>(_jsonOptions);

            return resultado?.Sucesso == true
                ? resultado.Dados ?? Enumerable.Empty<VendaDto>()
                : Enumerable.Empty<VendaDto>();
        }


        public async Task<IEnumerable<VendaDto>> ObterVendasPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            var response = await _httpClient.GetAsync($"api/vendas/periodo?inicio={inicio:o}&fim={fim:o}");
            response.EnsureSuccessStatusCode();

            var resultado = await response.Content.ReadFromJsonAsync<DTOs.ResultadoOperacao<IEnumerable<VendaDto>>>();

            return resultado?.Sucesso == true
                ? resultado.Dados ?? Enumerable.Empty<VendaDto>()
                : Enumerable.Empty<VendaDto>();
        }

        public async Task<VendaDto> CriarVendaAsync(CriarVendaDto venda)
        {
            var response = await _httpClient.PostAsJsonAsync("api/vendas", venda, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var vendaCriada = await response.Content.ReadFromJsonAsync<VendaDto>(_jsonOptions);
            return vendaCriada ?? throw new InvalidOperationException("Erro ao criar venda");
        }

        public async Task<VendaDto> AtualizarVendaAsync(AtualizarVendaDto venda)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/vendas/{venda.Id}", venda, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var vendaAtualizada = await response.Content.ReadFromJsonAsync<VendaDto>(_jsonOptions);
            return vendaAtualizada ?? throw new InvalidOperationException("Erro ao atualizar venda");
        }

        public async Task<List<VendaDto>> PesquisarVendasAsync(string nomeFuncionario, DateTime? dataInicio, DateTime? dataFim)

        {
            var url = $"api/vendas/pesquisar?termo={nomeFuncionario}&dataInicio={dataInicio}&dataFim={dataFim}";
            return await _httpClient.GetFromJsonAsync<List<VendaDto>>(url);

        }

        public async Task RemoverVendaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/vendas/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<EstatisticasVendasDto> ObterEstatisticasVendasAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            var response = await _httpClient.GetAsync($"api/vendas/estatisticas?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}");
            response.EnsureSuccessStatusCode();
            var estatisticas = await response.Content.ReadFromJsonAsync<EstatisticasVendasDto>(_jsonOptions);
            return estatisticas ?? new EstatisticasVendasDto();
        }



        #endregion

        #region Ranking

        public async Task<IEnumerable<RankingDto>> CalcularRankingAsync(FiltroRankingDto? filtro = null)
        {
            var url = "api/ranking";
            if (filtro != null)
            {
                var queryParams = new List<string>();

                if (filtro.DataInicio.HasValue)
                    queryParams.Add($"dataInicio={filtro.DataInicio.Value:yyyy-MM-dd}");

                if (filtro.DataFim.HasValue)
                    queryParams.Add($"dataFim={filtro.DataFim.Value:yyyy-MM-dd}");

                queryParams.Add($"tipoOrdenacao={filtro.TipoOrdenacao}");

                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);
            }

            var response = await _httpClient.GetAsync(url);
            var resultado = await TratarRespostaResultado<IEnumerable<RankingDto>>(response, "Erro ao calcular ranking.");

            return resultado.Sucesso ? resultado.Dados ?? Enumerable.Empty<RankingDto>() : Enumerable.Empty<RankingDto>();
        }

        public async Task<IEnumerable<RankingDto>> CalcularRankingPeriodoAtualAsync()
        {
            var response = await _httpClient.GetAsync("api/ranking/periodo-atual");
            var resultado = await TratarRespostaResultado<IEnumerable<RankingDto>>(response, "Erro ao calcular ranking do período atual.");

            return resultado.Sucesso ? resultado.Dados ?? Enumerable.Empty<RankingDto>() : Enumerable.Empty<RankingDto>();
        }

        public async Task<RankingDto?> ObterDesempenhoFuncionarioAsync(int funcionarioId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var url = $"api/ranking/funcionario/{funcionarioId}";
            var queryParams = new List<string>();

            if (dataInicio.HasValue)
                queryParams.Add($"dataInicio={dataInicio.Value:yyyy-MM-dd}");

            if (dataFim.HasValue)
                queryParams.Add($"dataFim={dataFim.Value:yyyy-MM-dd}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            var resultado = await TratarRespostaResultado<RankingDto>(response, "Erro ao obter desempenho do funcionário.");

            return resultado.Sucesso ? resultado.Dados : null;
        }


        public async Task<EstatisticasRankingDto> CalcularEstatisticasRankingAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var url = "api/ranking/estatisticas";
            var queryParams = new List<string>();

            if (dataInicio.HasValue)
                queryParams.Add($"dataInicio={dataInicio.Value:yyyy-MM-dd}");

            if (dataFim.HasValue)
                queryParams.Add($"dataFim={dataFim.Value:yyyy-MM-dd}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            var resultado = await TratarRespostaResultado<EstatisticasRankingDto>(response, "Erro ao calcular estatísticas.");

            return resultado.Sucesso ? resultado.Dados ?? new EstatisticasRankingDto() : new EstatisticasRankingDto();
        }

        public async Task<IEnumerable<RankingDto>> ObterFuncionariosQueAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var url = "api/ranking/atingiram-metas";
            var queryParams = new List<string>();

            if (dataInicio.HasValue)
                queryParams.Add($"dataInicio={dataInicio.Value:yyyy-MM-dd}");

            if (dataFim.HasValue)
                queryParams.Add($"dataFim={dataFim.Value:yyyy-MM-dd}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            var resultado = await TratarRespostaResultado<IEnumerable<RankingDto>>(response, "Erro ao obter funcionários que atingiram metas.");

            return resultado.Sucesso ? resultado.Dados ?? Enumerable.Empty<RankingDto>() : Enumerable.Empty<RankingDto>();
        }

        public async Task<IEnumerable<RankingDto>> ObterFuncionariosQueNaoAtingiramMetasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var url = "api/ranking/nao-atingiram-metas";
            var queryParams = new List<string>();

            if (dataInicio.HasValue)
                queryParams.Add($"dataInicio={dataInicio.Value:yyyy-MM-dd}");

            if (dataFim.HasValue)
                queryParams.Add($"dataFim={dataFim.Value:yyyy-MM-dd}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            var resultado = await TratarRespostaResultado<IEnumerable<RankingDto>>(response, "Erro ao obter funcionários que não atingiram metas.");

            return resultado.Sucesso ? resultado.Dados ?? Enumerable.Empty<RankingDto>() : Enumerable.Empty<RankingDto>();
        }

        public async Task<bool> VerificarMetasAtivasAsync(DateTime? dataInicio, DateTime? dataFim)
        {
            var url = $"api/ranking/metas-ativas?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}";

            try
            {
                var resultado = await _httpClient.GetFromJsonAsync<DTOs.ResultadoOperacao<bool>>(url);

                return resultado?.Sucesso == true && resultado.Dados == true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao verificar metas ativas: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
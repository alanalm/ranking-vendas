using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;
using RankingVendedores.Servicos.Interfaces;
using System.Net.Http.Json;

namespace RankingVendedores.Servicos.Api
{
    public class VendaApiService : ApiServiceBase, IVendaApiService
    {
        public VendaApiService(HttpClient httpClient) : base(httpClient) { }
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

            var resultado = await response.Content.ReadFromJsonAsync<ResultadoOperacao<IEnumerable<VendaDto>>>(_jsonOptions);

            return resultado?.Sucesso == true
                ? resultado.Dados ?? Enumerable.Empty<VendaDto>()
                : Enumerable.Empty<VendaDto>();
        }


        public async Task<IEnumerable<VendaDto>> ObterVendasPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            var response = await _httpClient.GetAsync($"api/vendas/periodo?inicio={inicio:o}&fim={fim:o}");
            response.EnsureSuccessStatusCode();

            var resultado = await response.Content.ReadFromJsonAsync<ResultadoOperacao<IEnumerable<VendaDto>>>();

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
    }
}

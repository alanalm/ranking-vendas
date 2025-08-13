using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;
using RankingVendedores.Servicos.Interfaces;
using System.Net.Http.Json;

namespace RankingVendedores.Servicos.Api
{
    public class IndicadorApiService : ApiServiceBase, IIndicadorApiService
    {
        public IndicadorApiService(HttpClient httpClient) : base(httpClient) { }
        public async Task<ResultadoOperacao<List<IndicadorDto>>> ObterIndicadoresAsync()
        {
            var response = await _httpClient.GetAsync("api/indicadores");
            return await TratarRespostaApi<List<IndicadorDto>>(response, "Erro ao obter indicadores.");
        }

        public async Task<ResultadoOperacao<IndicadorDto>> ObterIndicadorPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/indicadores/{id}");
            return await TratarRespostaApi<IndicadorDto>(response, $"Erro ao obter indicador com ID {id}.");
        }

        public async Task<ResultadoOperacao<IEnumerable<IndicadorDto>>> PesquisarIndicadoresAsync(string nome)
        {
            var response = await _httpClient.GetAsync($"api/indicadores/pesquisar?nome={Uri.EscapeDataString(nome)}");
            return await TratarRespostaApi<IEnumerable<IndicadorDto>>(response, "Erro ao pesquisar indicadores.");
        }

        public async Task<ResultadoOperacao<IndicadorDto>> CriarIndicadorAsync(CriarIndicadorDto indicador)
        {
            var response = await _httpClient.PostAsJsonAsync("api/indicadores", indicador, _jsonOptions);
            return await TratarRespostaApi<IndicadorDto>(response, "Erro ao criar indicador.");
        }

        public async Task<ResultadoOperacao<IndicadorDto>> AtualizarIndicadorAsync(AtualizarIndicadorDto indicador)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/indicadores/{indicador.Id}", indicador, _jsonOptions);
            return await TratarRespostaApi<IndicadorDto>(response, "Erro ao atualizar indicador.");
        }

        public async Task<ResultadoOperacao<bool>> RemoverIndicadorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/indicadores/{id}");
            return await TratarRespostaApi<bool>(response, "Erro ao remover indicador.");
        }

        public async Task<ResultadoOperacao<bool>> PodeIndicadorSerRemovidoAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/indicadores/{id}/pode-remover");
            return await TratarRespostaApi<bool>(response, "Erro ao verificar se o indicador pode ser removido.");
        }
    }
}

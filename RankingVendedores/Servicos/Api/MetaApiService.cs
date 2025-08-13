using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;
using RankingVendedores.Servicos.Interfaces;
using System.Net.Http.Json;

namespace RankingVendedores.Servicos.Api
{
    public class MetaApiService : ApiServiceBase, IMetaApiService
    {
        public MetaApiService(HttpClient httpClient) : base(httpClient) { }
        public async Task<List<MetaDto>> ObterMetasAsync()
        {
            var resultado = await ObterMetasComResultadoAsync();

            if (resultado.Sucesso && resultado.Dados != null)
                return resultado.Dados;

            return new List<MetaDto>();
        }

        public async Task<ResultadoOperacao<List<MetaDto>>> ObterMetasComResultadoAsync()
        {

            var response = await _httpClient.GetAsync("api/metas");

            var resultado = await TratarRespostaResultado<List<MetaDto>>(response, "Erro ao obter metas da API.");

            return resultado;
        }


        public async Task<ResultadoOperacao<MetaDto>> ObterMetaPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/metas/{id}");
            return await TratarRespostaApi<MetaDto>(response, $"Erro ao obter meta com ID {id}.");
        }

        public async Task<ResultadoOperacao<List<MetaDto>>> ObterMetasPorIndicadorAsync(int indicadorId)
        {
            var response = await _httpClient.GetAsync($"api/metas/indicador/{indicadorId}");
            return await TratarRespostaApi<List<MetaDto>>(response, $"Erro ao obter metas do indicador {indicadorId}.");
        }

        public async Task<ResultadoOperacao<MetaDto>> CriarMetaAsync(CriarMetaDto meta)
        {
            var response = await _httpClient.PostAsJsonAsync("api/metas", meta, _jsonOptions);
            return await TratarRespostaApi<MetaDto>(response, "Erro ao criar meta.");
        }

        public async Task<ResultadoOperacao<MetaDto>> AtualizarMetaAsync(AtualizarMetaDto meta)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/metas/{meta.Id}", meta, _jsonOptions);
            return await TratarRespostaApi<MetaDto>(response, "Erro ao atualizar meta.");
        }

        public async Task RemoverMetaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/metas/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<ResultadoOperacao<bool>> AtivarMetaAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/metas/{id}/ativar", null);
            return await TratarRespostaApi<bool>(response, "Erro ao ativar meta.");
        }

        public async Task<ResultadoOperacao<bool>> DesativarMetaAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/metas/{id}/desativar", null);
            return await TratarRespostaApi<bool>(response, "Erro ao desativar meta.");
        }
    }
}

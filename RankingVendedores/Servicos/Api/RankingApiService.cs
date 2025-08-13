using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;
using RankingVendedores.Servicos.Interfaces;
using System.Net.Http.Json;

namespace RankingVendedores.Servicos.Api
{
    public class RankingApiService : ApiServiceBase, IRankingApiService
    {
        public RankingApiService(HttpClient httpClient) : base(httpClient) { }
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
                var resultado = await _httpClient.GetFromJsonAsync<ResultadoOperacao<bool>>(url);

                return resultado?.Sucesso == true && resultado.Dados == true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao verificar metas ativas: {ex.Message}");
                return false;
            }
        }
    }
}

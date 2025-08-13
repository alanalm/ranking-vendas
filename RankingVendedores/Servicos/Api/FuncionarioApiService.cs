using Aplicacao.Utils;
using Ranking.Aplicacao.DTOs;
using RankingVendedores.Servicos.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace RankingVendedores.Servicos.Api
{
    public class FuncionarioApiService : ApiServiceBase, IFuncionarioApiService
    {
        public FuncionarioApiService(HttpClient httpClient) : base(httpClient) { }

        public async Task<ResultadoOperacao<List<FuncionarioDto>>> ObterFuncionariosAsync()
        {
            var response = await _httpClient.GetAsync("api/funcionarios");

            if (!response.IsSuccessStatusCode)
                return new ResultadoOperacao<List<FuncionarioDto>>
                {
                    Sucesso = false,
                    Mensagem = "Erro ao consultar a API.",
                    Erros = new List<string> { "Erro ao consultar a API." }
                };

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var resultado = await response.Content.ReadFromJsonAsync<ResultadoOperacao<List<FuncionarioDto>>>(options);

            return resultado ?? new ResultadoOperacao<List<FuncionarioDto>>
            {
                Sucesso = false,
                Mensagem = "Resposta nula.",
                Erros = new List<string> { "Resposta nula." }
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

            var resultado = await response.Content.ReadFromJsonAsync<ResultadoOperacao<List<FuncionarioDto>>>();

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

                var resultado = await response.Content.ReadFromJsonAsync<ResultadoOperacao<bool>>();
                return resultado?.Dados ?? false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar remoção: {ex.Message}");
                return false;
            }
        }
    }
}
